namespace PortfolioSaaS.Domain.Entities;

public class Color
{
    public string PrimaryColor { get; set; } = "#1976d2";
    public string SecondaryColor { get; set; } = "#546e7a";
    public string BackgroundColor { get; set; } = "#ffffff";
    public string SurfaceColor { get; set; } = "#f5f5f5";
    public string TextColor { get; set; } = "#1e1e1e";
    public string TextSecondaryColor { get; set; } = "#616161";
    public string FontFamily { get; set; } = "Inter, system-ui, -apple-system, Segoe UI, Roboto, sans-serif";
    public string BorderRadius { get; set; } = "0.5rem";
}
