using Ardalis.Specification;
using PortfolioSaaS.Domain.Entities;

namespace PortfolioSaaS.Infrastructure.Specifications;

public static class UserSpecs
{
    public static Specification<User> ByEmail(string email)
    {
        var spec = new Specification<User>();
        spec.Query.Where(x => x.Email == email);
        return spec;
    }
}
