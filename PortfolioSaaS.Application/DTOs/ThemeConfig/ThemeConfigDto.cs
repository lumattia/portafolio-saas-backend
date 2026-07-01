using PortfolioSaaS.Domain.Entities;

namespace PortfolioSaaS.Application.DTOs.ThemeConfig;

public class ThemeConfigDto
{
    public Guid Id { get; set; }
    public ColorDto Light { get; set; } = new();
    public ColorDto Dark { get; set; } = new();
}

public class ColorDto
{
    public string PrimaryColor { get; set; } = "#6366f1";
    public string SecondaryColor { get; set; } = "#8b5cf6";
    public string BackgroundColor { get; set; } = "#ffffff";
    public string SurfaceColor { get; set; } = "#f8fafc";
    public string TextColor { get; set; } = "#0f172a";
    public string TextSecondaryColor { get; set; } = "#64748b";
    public string FontFamily { get; set; } = "Inter, system-ui, sans-serif";
    public string BorderRadius { get; set; } = "0.75rem";
}
