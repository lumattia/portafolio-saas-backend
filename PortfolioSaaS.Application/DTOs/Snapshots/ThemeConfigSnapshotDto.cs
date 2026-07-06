using System.Text.Json;
using PortfolioSaaS.Domain.Entities;

namespace PortfolioSaaS.Application.DTOs.Snapshots;

public class ThemeConfigSnapshotDto
{
    public Guid Id { get; set; }
    public Color Light { get; set; } = new();
    public Color Dark { get; set; } = new();
}
