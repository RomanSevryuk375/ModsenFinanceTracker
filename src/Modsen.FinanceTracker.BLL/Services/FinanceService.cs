using Modsen.FinanceTracker.BLL.DTOs;
using Modsen.FinanceTracker.BLL.Interfaces;
using Modsen.FinanceTracker.Domain.Entities;
using Modsen.FinanceTracker.Domain.Interfaces;

namespace Modsen.FinanceTracker.BLL.Services;

public class FinanceService : IFinanceService
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

    public async Task UpdateTransactionAsync(Transaction transaction, CancellationToken ct = default)
    {
        await _repository.UpdateAsync(transaction, ct);
    }

    public async Task<decimal> GetBalanceAsync(CancellationToken ct = default)
    {
        var transactions = await _repository.GetAllAsync(ct: ct);

        return transactions.Sum(t => t is IncomeTransaction ? t.Amount : -t.Amount);
    }

    public async Task<IEnumerable<Transaction>> GetFilteredTransactionsAsync(
        TransactionFilterDto filter,
        CancellationToken ct = default)
    {
        return await _repository.GetAllAsync(filter.ToExpression(), ct: ct);
    }
}