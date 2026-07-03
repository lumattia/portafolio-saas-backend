using PortfolioSaaS.Domain.Entities;

namespace PortfolioSaaS.Application.DTOs.ThemeConfig;

public class ThemeConfigDto
{
    public Guid Id { get; set; }
    public Color Light { get; set; } = new();
    public Color Dark { get; set; } = new();
}