namespace PortfolioSaaS.Domain.Entities;

[Flags]
public enum SectionCategory
{
    None = 0,
    Text = 1,
    Image = 2,
    Container = 4,
}

public class SectionTemplate
{
    public Guid Id { get; set; }
    public string ComponentSelector { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public SectionCategory CategoryTags { get; set; }
    public string? PreviewImageUrl { get; set; }
    public string DefaultContentJson { get; set; } = "{}";
}
