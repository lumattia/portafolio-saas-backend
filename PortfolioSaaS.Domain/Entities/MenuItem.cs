namespace PortfolioSaaS.Domain.Entities;

public class MenuItem
{
    public Guid Id { get; set; }
    public string Text { get; set; } = string.Empty;
    public Guid MenuId { get; set; }
    public string Url { get; set; } = string.Empty;
    public int Order { get; set; }
}
