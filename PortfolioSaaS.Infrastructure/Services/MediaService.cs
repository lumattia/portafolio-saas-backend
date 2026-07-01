using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace PortfolioSaaS.Infrastructure.Services;

public class MediaService
{
    private static readonly string[] AllowedContentTypes =
    [
        "image/jpeg",
        "image/jpg",
        "image/png",
        "image/webp"
    ];

    private readonly IWebHostEnvironment _environment;

    public MediaService(IWebHostEnvironment environment)
    {
        _environment = environment;
    }

    public async Task<string> SaveImageAsync(IFormFile file, Guid tenantId, CancellationToken cancellationToken = default)
    {
        if (file.Length == 0)
            throw new InvalidOperationException("Empty file.");

        if (file.Length > 5 * 1024 * 1024)
            throw new InvalidOperationException("File exceeds 5 MB limit.");

        if (!AllowedContentTypes.Contains(file.ContentType.ToLowerInvariant()))
            throw new InvalidOperationException("Unsupported image format.");

        var extension = Path.GetExtension(file.FileName);
        if (string.IsNullOrWhiteSpace(extension))
            extension = file.ContentType switch
            {
                "image/png" => ".png",
                "image/webp" => ".webp",
                _ => ".jpg"
            };

        var uploadsRoot = Path.Combine(_environment.WebRootPath, "uploads", tenantId.ToString());
        Directory.CreateDirectory(uploadsRoot);

        var fileName = $"{Guid.NewGuid()}{extension.ToLowerInvariant()}";
        var fullPath = Path.Combine(uploadsRoot, fileName);

        await using var stream = File.Create(fullPath);
        await file.CopyToAsync(stream, cancellationToken);

        return $"/uploads/{tenantId}/{fileName}";
    }
}
