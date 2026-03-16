using Modsen.FinanceTracker.BLL.DTOs;
using Modsen.FinanceTracker.Domain.Entities;
using Modsen.FinanceTracker.Domain.Interfaces;

namespace Modsen.FinanceTracker.BLL.Services;

public class FinanceService
{
    private readonly IRepository<Transaction> _repository;

    public FinanceService(IRepository<Transaction> repository)
    {
        _repository = repository;
    }

    public async Task AddTransactionAsync(Transaction transaction, CancellationToken ct = default)
    {
        await _repository.AddAsync(transaction, ct);
    }

    public async Task DeleteTransactionAsync(Guid id, CancellationToken ct = default)
    {
        await _repository.DeleteAsync(id, ct);
    }

    public async Task<decimal> GetBalanceAsync()
    {
        var transactions = await _repository.GetAllAsync();
        
        return transactions.Sum(t => t is IncomeTransaction ? t.Amount : -t.Amount);
    }
    
    public async Task<IEnumerable<Transaction>> GetFilteredTransactionsAsync(TransactionFilterDto filter)
    {
        return await _repository.GetAllAsync(filter.ToExpression());
    }
}