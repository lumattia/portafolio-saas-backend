using System.Text.Json;
using PortfolioSaaS.Domain.Entities;

namespace PortfolioSaaS.Application.DTOs.Snapshots;

public class MenuSnapshotDto
{
    public MenuType Type { get; set; }
    // Navigation
    public JsonDocument ContentJson { get; set; } = JsonDocument.Parse("[]");
}
