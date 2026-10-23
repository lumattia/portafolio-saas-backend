using PortfolioSaaS.Domain.Common;

namespace PortfolioSaaS.Domain.Entities;

public enum MediaType
{
    Image,
    File
}

public class Media : ITenantEntity
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string Key { get; set; } = string.Empty;
    public string MimeType { get; set; } = string.Empty;
    public long Size { get; set; }
    public string Url { get; set; } = string.Empty;
    public MediaType MediaType { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public void SetMediaTypeFromMimeType()
    {
        MediaType = MimeType.StartsWith("image/") ? MediaType.Image : MediaType.File;
    }
}
