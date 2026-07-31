namespace PortfolioSaaS.Infrastructure.Services;
public interface ITransactionParticipant
{
    void BeginTransaction(CancellationToken cancellationToken = default);
    Task CommitAsync(CancellationToken cancellationToken = default);
    Task AfterCommitAsync(CancellationToken cancellationToken = default);
    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
    void ClearAll();
}