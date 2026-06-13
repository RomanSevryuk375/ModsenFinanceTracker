using Modsen.FinanceTracker.BLL.DTOs;
using Modsen.FinanceTracker.Domain.Entities;

namespace Modsen.FinanceTracker.BLL.Interfaces;

public interface IFinanceService
{
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