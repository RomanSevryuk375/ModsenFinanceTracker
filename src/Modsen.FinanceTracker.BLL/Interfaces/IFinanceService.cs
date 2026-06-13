using Modsen.FinanceTracker.BLL.DTOs;
using Modsen.FinanceTracker.Domain.Entities;
using Modsen.FinanceTracker.Domain.Events;

namespace Modsen.FinanceTracker.BLL.Interfaces;

public interface IFinanceService
{
    event EventHandler<CategoryLimitExceededEventArgs>? OnCategoryLimitExceeded;
    Task AddTransactionAsync(
        Transaction transaction, 
        CancellationToken cancellationToken = default);

    Task DeleteTransactionAsync(
        Guid id, 
        CancellationToken cancellationToken = default);

    Task<decimal> GetBalanceAsync(
        CancellationToken cancellationToken = default);

    Task<IEnumerable<Transaction>> GetFilteredTransactionsAsync(
        TransactionFilterDto filter, 
        CancellationToken cancellationToken = default);

    Task UpdateTransactionAsync(
        Transaction transaction, 
        CancellationToken cancellationToken = default);
}