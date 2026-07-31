using Ardalis.Specification;
using AutoMapper;
using PortfolioSaaS.Application.DTOs.ThemeConfig;
using PortfolioSaaS.Domain.Entities;
using PortfolioSaaS.Infrastructure.Data;
using PortfolioSaaS.Infrastructure.Specifications;

namespace PortfolioSaaS.Infrastructure.Services;

public class UnitOfWork(ApplicationDbContext context, FileStorageService fileStorageService, IMapper mapper)
{
    private readonly ApplicationDbContext _context = context;
    private readonly IFileStorageTransaction _fileStorageTransaction = fileStorageService;
    private bool _transactionStarted = false;
    public async virtual Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transactionStarted)
            throw new InvalidOperationException("Transaction already started");
        _fileStorageTransaction.BeginTransaction(cancellationToken);
        await _context.Database.BeginTransactionAsync(cancellationToken);
        _transactionStarted = true;
    }
     public virtual async Task CommitAsync(CancellationToken cancellationToken = default)
    {
        if (!_transactionStarted)
            throw new InvalidOperationException("Transaction not started. Call BeginTransaction first.");


        try
        {
            await _fileStorageTransaction.CommitAsync(cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            await _context.Database.CommitTransactionAsync(cancellationToken);
            await _fileStorageTransaction.AfterCommitAsync(cancellationToken);
            return;
        }
        catch (Exception)
        {
            await RollbackTransactionAsync(cancellationToken);
            throw;
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
            await _fileStorageTransaction.RollbackTransactionAsync(cancellationToken);
        }
    }
}
