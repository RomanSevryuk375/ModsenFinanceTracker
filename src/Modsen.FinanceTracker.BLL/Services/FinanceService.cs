using Modsen.FinanceTracker.BLL.DTOs;
using Modsen.FinanceTracker.BLL.Interfaces;
using Modsen.FinanceTracker.Domain.Entities;
using Modsen.FinanceTracker.Domain.Interfaces;

namespace Modsen.FinanceTracker.BLL.Services;

public sealed class FinanceService(
    IRepository<Transaction> repository, 
    IValidator<Transaction> validator) : IFinanceService
{
    public async Task AddTransactionAsync(
        Transaction transaction, 
        CancellationToken cancellationToken = default)
    {
        var (isValid, message) = validator.Validate(transaction);
        if (!isValid)
        {
            throw new ArgumentException(message);
        }

        await repository.AddAsync(transaction, cancellationToken);
    }

    public async Task DeleteTransactionAsync(
        Guid id, 
        CancellationToken cancellationToken = default)
    {
        await repository.DeleteAsync(id, cancellationToken);
    }

    public async Task UpdateTransactionAsync(
        Transaction transaction, 
        CancellationToken cancellationToken = default)
    {
        await repository.UpdateAsync(transaction, cancellationToken);
    }

    public async Task<decimal> GetBalanceAsync(
        CancellationToken cancellationToken = default)
    {
        var transactions = await repository.GetAllAsync(
            cancellationToken: cancellationToken);

        return transactions.Sum(t => 
            t is IncomeTransaction 
                ? t.Amount 
                : -t.Amount);
    }

    public async Task<IEnumerable<Transaction>> GetFilteredTransactionsAsync(
        TransactionFilterDto filter,
        CancellationToken cancellationToken = default)
    {
        return await repository.GetAllAsync(
            filter.ToExpression(), cancellationToken: cancellationToken);
    }
}