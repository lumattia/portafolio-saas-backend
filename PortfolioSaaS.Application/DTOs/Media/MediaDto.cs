namespace PortfolioSaaS.Application.DTOs.Media;

public class MediaDto
{
    public Guid Id { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string MediaType { get; set; } = string.Empty;
    public long Size { get; set; }
    public string Url { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class MediaFilterRequest
{
    public string? SearchTerm { get; set; }
    public string? MediaType { get; set; }
}
