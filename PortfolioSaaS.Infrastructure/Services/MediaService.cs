using AutoMapper;
using Microsoft.AspNetCore.Http;
using PortfolioSaaS.Application.DTOs.Media;
using PortfolioSaaS.Application.DTOs.PagedList;
using PortfolioSaaS.Domain.Common;
using PortfolioSaaS.Domain.Entities;
using PortfolioSaaS.Infrastructure.Data;
using PortfolioSaaS.Infrastructure.Specifications;

namespace PortfolioSaaS.Infrastructure.Services;

public class MediaService
{
    private readonly BaseRepository<Media> _mediaRepository;
    private readonly TenantContext _tenantContext;
    private readonly FileStorageService _fileStorageService;
    private readonly UnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public MediaService(
        BaseRepository<Media> mediaRepository,
        TenantContext tenantContext,
        FileStorageService fileStorageService,
        UnitOfWork unitOfWork,
        IMapper mapper)
    {
        _mediaRepository = mediaRepository;
        _tenantContext = tenantContext;
        _fileStorageService = fileStorageService;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<PagedList<MediaDto>> GetMediaAsync(
        MediaFilterRequest request,
        PagedParameters parameters,
        CancellationToken cancellationToken = default)
    {
        var spec = MediaSpecs.Page(request, parameters, _tenantContext.CurrentTenantId!.Value);
        var pagedResult = await _mediaRepository.PageBySpecAsync(spec, cancellationToken);
        return _mapper.Map<PagedList<MediaDto>>(pagedResult);
    }

    public async Task<MediaDto> CreateMediaAsync(
        IFormFile file,
        CancellationToken cancellationToken = default)
    {
        if (!_tenantContext.IsResolved)
            throw new InvalidOperationException("Tenant context required.");

        if (file is null || file.Length == 0)
            throw new InvalidOperationException("No file provided.");

        _unitOfWork.RegisterParticipant(_fileStorageService);
        await _unitOfWork.BeginTransactionAsync(cancellationToken);

        try
        {
            var mediaFolder = $"uploads/{_tenantContext.CurrentTenantId!.Value}/media";
            var key = _fileStorageService.QueueUpload(file, mediaFolder, cancellationToken);

            var media = new Media
            {
                Id = Guid.NewGuid(),
                TenantId = _tenantContext.CurrentTenantId!.Value,
                FileName = file.FileName,
                Key = key,
                MimeType = file.ContentType,
                Size = file.Length,
                Url = _fileStorageService.GeneratePublicUrl(key),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            media.SetMediaTypeFromMimeType();

            await _mediaRepository.SaveAsync(media, cancellationToken);
            await _unitOfWork.CommitAsync(cancellationToken);

            return _mapper.Map<MediaDto>(media);
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            throw;
        }
    }

    public async Task<MediaDto?> UpdateMediaAsync(
        Guid id,
        IFormFile file,
        CancellationToken cancellationToken = default)
    {
        var media = await _mediaRepository.GetByIdAsync(id, cancellationToken);

        if (media is null)
            return null;

        if (file is not null && file.Length > 0)
        {
            _unitOfWork.RegisterParticipant(_fileStorageService);
            await _unitOfWork.BeginTransactionAsync(cancellationToken);

            try
            {
                // Delete old file
                _fileStorageService.QueueDeleteLog(media.Key, cancellationToken);

                // Upload new file
                var mediaFolder = $"uploads/{_tenantContext.CurrentTenantId!.Value}/media";
                var key = _fileStorageService.QueueUpload(file, mediaFolder, cancellationToken);

                media.FileName = file.FileName;
                media.Key = key;
                media.MimeType = file.ContentType;
                media.Size = file.Length;
                media.Url = _fileStorageService.GeneratePublicUrl(key);
                media.UpdatedAt = DateTime.UtcNow;

                media.SetMediaTypeFromMimeType();

                await _mediaRepository.SaveAsync(media, cancellationToken);
                await _unitOfWork.CommitAsync(cancellationToken);

                return _mapper.Map<MediaDto>(media);
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                throw;
            }
        }
        else
        {
            // No file update, just metadata
            media.UpdatedAt = DateTime.UtcNow;
            await _mediaRepository.SaveAsync(media, cancellationToken);
            return _mapper.Map<MediaDto>(media);
        }
    }

    public async Task DeleteMediaAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var media = await _mediaRepository.GetByIdAsync(id, cancellationToken);

        if (media is null)
            throw new InvalidOperationException("Media not found.");

        // Check if media is in use by sections (TODO: Add tracker for media usage)
        // For now, throw error if media is in use
        // Future: Add confirmation dialog with tracker showing usage count and locations

        _unitOfWork.RegisterParticipant(_fileStorageService);
        await _unitOfWork.BeginTransactionAsync(cancellationToken);

        try
        {
            // Delete physical file
            _fileStorageService.QueueDeleteLog(media.Key, cancellationToken);

            await _mediaRepository.DeleteAsync(media, cancellationToken);
            await _unitOfWork.CommitAsync(cancellationToken);
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            throw;
        }
    }

    // TODO: Future features to add:
    // - FileSelector component for non-image files
    // - MediaSelector that shows available selector types (ImageSelector, FileSelector, etc.)
    // - Different view layouts for file types
    // - Drag and drop functionality
    // - Advanced filtering options
    // - Media usage tracker
    // - Bulk operations
}