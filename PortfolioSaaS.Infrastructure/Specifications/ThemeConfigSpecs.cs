using Ardalis.Specification;
using PortfolioSaaS.Domain.Entities;

namespace PortfolioSaaS.Infrastructure.Specifications;

public class ThemeConfigSpecs : Specification<ThemeConfig>
{
    public static ThemeConfigSpecs Get()
    {
        var spec = new ThemeConfigSpecs();
        return spec;
    }
}
