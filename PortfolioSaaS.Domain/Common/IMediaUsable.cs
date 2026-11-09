using PortfolioSaaS.Domain.Entities;

namespace PortfolioSaaS.Domain.Common;

public interface IMediaUsable
{
    public Guid? MediaId { get; set; }
    public Media? Media { get; set; }
    string GetUsageRoute();
}