using PortfolioSaaS.Domain.Common;

namespace PortfolioSaaS.Application.DTOs.Versions;

public class VersionListDto
{
    public Guid? CurrentVersionId { get; set; }
    public List<IdName> Versions { get; set; } = [];
}
