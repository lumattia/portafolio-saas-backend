namespace PortfolioSaaS.Domain.Common;

public interface IIdName
{
    Guid Id { get; }
    string Name { get; }
}

public record IdName(Guid Id, string Name) : IIdName;
