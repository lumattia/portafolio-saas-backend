using AutoMapper;
using PortfolioSaaS.Infrastructure.Data;

namespace PortfolioSaaS.Infrastructure.Services;

public class UnitOfWork(ApplicationDbContext context)
{
    private readonly ApplicationDbContext _context = context;
    private readonly List<ITransactionParticipant> _participants = [];
    private bool _transactionStarted = false;
    public void RegisterParticipant(ITransactionParticipant participant)
    {
        if (!_participants.Contains(participant))
        {
            _participants.Add(participant);
        }
    }
    public async virtual Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transactionStarted)
            throw new InvalidOperationException("Transaction already started");
        foreach (var participant in _participants)
        {
            participant.BeginTransaction(cancellationToken);
        }
        await _context.Database.BeginTransactionAsync(cancellationToken);
        _transactionStarted = true;
    }
     public virtual async Task CommitAsync(CancellationToken cancellationToken = default)
    {
        if (!_transactionStarted)
            throw new InvalidOperationException("Transaction not started. Call BeginTransaction first.");

        try
        {
            foreach (var participant in _participants)
            {
                await participant.CommitAsync(cancellationToken);
            }
            await _context.SaveChangesAsync(cancellationToken);
            await _context.Database.CommitTransactionAsync(cancellationToken);
            foreach (var participant in _participants)
            {
                await participant.AfterCommitAsync(cancellationToken);
            }
            return;
        }
        catch (Exception)
        {
            await RollbackTransactionAsync(cancellationToken);
            throw;
        }
        finally
        {
            foreach (var participant in _participants)
            {
                participant.ClearAll();
            }
            _transactionStarted = false;
        }
    }
      public virtual async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await _context.Database.RollbackTransactionAsync(cancellationToken);
        }
        finally
        {
            foreach (var participant in _participants)
            {
                try
                {
                    await participant.RollbackTransactionAsync(cancellationToken);
                }
                catch
                {
                    // Preventing a rollback failure in one participant stops the rollback of others.
                }
            }
        }
    }
}
