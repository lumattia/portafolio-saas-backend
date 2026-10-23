using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using PortfolioSaaS.Domain.Entities;
using PortfolioSaaS.Infrastructure.Configuration;
using PortfolioSaaS.Infrastructure.Data;

namespace PortfolioSaaS.Infrastructure.Services;

public class FileStorageService : ITransactionParticipant
{
    private readonly IAmazonS3 _s3Client;
    private readonly R2Settings _r2Settings;
    private readonly ILogger<FileStorageService> _logger;

    private readonly List<PutObjectRequest> _pendingUploads = new();
    private readonly List<CopyObjectRequest> _pendingCopies = new();
    private readonly List<string> _pendingDeletes = new();
    private readonly List<string> _uploadedInTransactionsKey = new();
    private bool _transactionStarted = false;

    public FileStorageService(
        IAmazonS3 s3Client,
        R2Settings r2Settings,
        ILogger<FileStorageService> logger)
    {
        _s3Client = s3Client;
        _r2Settings = r2Settings;
        _logger = logger;
    }
    void ITransactionParticipant.ClearAll()
    {
        _pendingUploads.Clear();
        _pendingDeletes.Clear();
        _pendingCopies.Clear();
        _uploadedInTransactionsKey.Clear();
        _transactionStarted = false;
    }
    void ITransactionParticipant.BeginTransaction(CancellationToken cancellationToken = default)
    {
        if (_transactionStarted) throw new InvalidOperationException("Transaction already started");
        _transactionStarted = true;
    }

    async Task ITransactionParticipant.CommitAsync(CancellationToken cancellationToken = default)
    {
        if (!_transactionStarted)
            throw new InvalidOperationException("FileStorageService transaction not started. Call BeginTransaction first.");

        try
        {
            foreach (var upload in _pendingUploads)
            {
                await _s3Client.PutObjectAsync(upload, cancellationToken);
                _uploadedInTransactionsKey.Add(upload.Key);
            }
            foreach (var copy in _pendingCopies)
            {
                await _s3Client.CopyObjectAsync(copy, cancellationToken);
                _uploadedInTransactionsKey.Add(copy.DestinationKey);
            }
            return;
        }
        catch (Exception e)
        {
            throw;
        }
    }
    async Task ITransactionParticipant.AfterCommitAsync(CancellationToken cancellationToken = default)
    {
        if (!_transactionStarted)
            throw new InvalidOperationException("FileStorageService transaction not started. Call BeginTransaction first.");
        await DeleteFileLogAsync(_pendingDeletes, cancellationToken);
    }

    async Task ITransactionParticipant.RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (!_transactionStarted)
            throw new InvalidOperationException("FileStorageService transaction not started. Call BeginTransaction first.");

        await DeleteFileLogAsync(_uploadedInTransactionsKey, cancellationToken);
    }

    public virtual string QueueUpload(
        IFormFile file,
        string folderPath,
        CancellationToken cancellationToken = default)
    {
        if (!_transactionStarted)
            throw new InvalidOperationException("FileStorageService transaction not started. Call BeginTransaction first.");

        if (file == null || file.Length == 0)
            throw new ArgumentException("File cannot be null or empty", nameof(file));

        var extension = Path.GetExtension(file.FileName);
        var contentType = file.ContentType ?? "application/octet-stream";
        var newKey = $"{folderPath}/{Guid.NewGuid()}{extension}";

        try
        {
            var memoryStream = new MemoryStream();
            file.CopyTo(memoryStream);
            memoryStream.Position = 0;

            var putRequest = new PutObjectRequest
            {
                BucketName = _r2Settings.BucketName,
                Key = newKey,
                ContentType = contentType,
                InputStream = memoryStream,
                DisablePayloadSigning = true
            };
            _pendingUploads.Add(putRequest);

            return newKey;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error uploading file to R2/S3");
            throw new InvalidOperationException("Error uploading file to storage", ex);
        }
    }

    public virtual void QueueDeleteLog(string key, CancellationToken cancellationToken = default)
    {
        if (!_transactionStarted)
            throw new InvalidOperationException("FileStorageService transaction not started. Call BeginTransaction first.");

        if (string.IsNullOrWhiteSpace(key)) return;
        _pendingDeletes.Add(key);
    }
    private async Task DeleteFileLogAsync(List<string> keys, CancellationToken cancellationToken = default)
    {
        if (!_transactionStarted)
            throw new InvalidOperationException("FileStorageService transaction not started. Call BeginTransaction first.");
        if (keys == null || keys.Count == 0) return;

        try
        {
            var sanitizedKeys = keys
                .Where(k => !string.IsNullOrWhiteSpace(k))
                .Select(k => k.TrimStart('/'))
                .ToList();

            if (sanitizedKeys.Count == 0) return;
            var request = new DeleteObjectsRequest
                {
                    BucketName = _r2Settings.BucketName,
                    Objects = sanitizedKeys.Select(key => new KeyVersion { Key = key }).ToList()
                };
            await _s3Client.DeleteObjectsAsync(request, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Error deleting files: {FileKey}", string.Join(", ", keys));
        }
    }
    public string GeneratePublicUrl(string key)
    {
        return $"{_r2Settings.PublicUrl.TrimEnd('/')}/{key}";
    }
}
