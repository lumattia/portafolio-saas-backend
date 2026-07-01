using Ardalis.Specification;
using PortfolioSaaS.Domain.Entities;

namespace PortfolioSaaS.Infrastructure.Specifications;

public class UserSpecs : Specification<User>
{
    public static UserSpecs ByEmail(string email)
    {
        var spec = new UserSpecs();
        spec.Query.Where(x => x.Email == email);
        return spec;
    }
}
