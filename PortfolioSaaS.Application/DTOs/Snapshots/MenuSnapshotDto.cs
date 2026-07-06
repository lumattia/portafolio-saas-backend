using System.Text.Json;
using PortfolioSaaS.Domain.Entities;

namespace PortfolioSaaS.Application.DTOs.Snapshots;

public class MenuSnapshotDto
{
    public MenuType Type { get; set; }
    // Navigation
    public JsonDocument MenuItems { get; set; } = JsonDocument.Parse("[]");
}
