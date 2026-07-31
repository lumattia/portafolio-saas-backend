using Amazon.S3;
using Amazon.S3.Model;
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

    private class FileUploadOperation
    {
        public string FileName { get; set; } = string.Empty;
        public string ContentType { get; set; } = string.Empty;
        public string Base64 { get; set; } = string.Empty;
        public string FolderPath { get; set; } = string.Empty;
    }

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

    public virtual byte[] ParseBase64(string base64)
    {
        if (base64.Contains(','))
        {
            base64 = base64.Split(',')[1];
        }
        return Convert.FromBase64String(base64);
    }

    public virtual FileReference QueueUpload(
        string fileName,
        string contentType,
        string base64Data,
        string folderPath,
        CancellationToken cancellationToken = default)
    {if (!_transactionStarted)
            throw new InvalidOperationException("FileStorageService transaction not started. Call BeginTransaction first.");

        var extension = "";
        var lastDotIndex = fileName.LastIndexOf('.');
        if (lastDotIndex > 0)
        {
            extension = fileName.Substring(lastDotIndex);
        }

        var newKey = $"{folderPath}/{Guid.NewGuid()}{extension}";

        try
        {
            var bytes = ParseBase64(base64Data);

            var putRequest = new PutObjectRequest
            {
                BucketName = _r2Settings.BucketName,
                Key = newKey,
                ContentType = contentType,
                InputStream = new MemoryStream(bytes),
                DisablePayloadSigning = true
            };
            _pendingUploads.Add(putRequest);


            var fileReference = new FileReference
            {
                Key = newKey,
                FileName = fileName,
                MimeType = contentType,
                Size = bytes.Length,
                Url = GeneratePublicUrl(newKey)
            };

            return fileReference;
        }
        catch (FormatException ex)
        {
            throw new InvalidOperationException("Invalid Base64 format", ex);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error uploading file to R2/S3");
            throw new InvalidOperationException("Error uploading file to storage", ex);
        }
    }

    public virtual void QueueDeleteLog(FileReference? fileReference, CancellationToken cancellationToken = default)
    {if (!_transactionStarted)
            throw new InvalidOperationException("FileStorageService transaction not started. Call BeginTransaction first.");

        if (fileReference == null) return;
        _pendingDeletes.Add(fileReference.Key);
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
    public virtual async Task SyncFolderAsync(
        string sourceFolderPath,
        string destinationFolderPath,
        CancellationToken cancellationToken = default)
    {if (!_transactionStarted)
            throw new InvalidOperationException("FileStorageService transaction not started. Call BeginTransaction first.");

        try
        {
            // Get all files in source folder
            var sourceListRequest = new ListObjectsV2Request
            {
                BucketName = _r2Settings.BucketName,
                Prefix = sourceFolderPath + "/"
            };

            var sourceResponse = await _s3Client.ListObjectsV2Async(sourceListRequest, cancellationToken);
            var sourceFiles = sourceResponse.S3Objects.Select(o => o.Key).ToHashSet();

            // Get all files in destination folder
            var destListRequest = new ListObjectsV2Request
            {
                BucketName = _r2Settings.BucketName,
                Prefix = destinationFolderPath + "/"
            };

            var destResponse = await _s3Client.ListObjectsV2Async(destListRequest, cancellationToken);
            var destFiles = destResponse.S3Objects.Select(o => o.Key).ToHashSet();

            // Calculate files to copy (in source but not in destination)
            var filesToCopy = sourceFiles.Where(f => !destFiles.Contains(
                $"{destinationFolderPath}/{f.Substring(sourceFolderPath.Length + 1)}")).ToList();

            // Calculate files to delete (in destination but not in source)
            var filesToDelete = destFiles.Where(f => !sourceFiles.Contains(
                $"{sourceFolderPath}/{f.Substring(destinationFolderPath.Length + 1)}")).ToList();

            // Copy new files
            if (filesToCopy.Count > 0)
            {
                _pendingCopies.AddRange(filesToCopy.Select(sourceKey =>
                {
                    var relativePath = sourceKey.Substring(sourceFolderPath.Length + 1);
                    var destinationKey = $"{destinationFolderPath}/{relativePath}";

                    return new CopyObjectRequest
                    {
                        SourceBucket = _r2Settings.BucketName,
                        SourceKey = sourceKey,
                        DestinationBucket = _r2Settings.BucketName,
                        DestinationKey = destinationKey
                    };
                }));
            }

            // Delete files that are no longer in source
            _pendingDeletes.AddRange(filesToDelete);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error syncing folder from {SourceFolder} to {DestFolder}",
                sourceFolderPath, destinationFolderPath);
            throw new InvalidOperationException($"Error syncing folder from {sourceFolderPath} to {destinationFolderPath}", ex);
        }
    }

    private string GeneratePublicUrl(string key)
    {
        return $"{_r2Settings.PublicUrl.TrimEnd('/')}/{key}";
    }
}
