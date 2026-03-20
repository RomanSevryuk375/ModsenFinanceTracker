using Modsen.FinanceTracker.BLL.DTOs;
using Modsen.FinanceTracker.Domain.Entities;

namespace Modsen.FinanceTracker.BLL.Interfaces
{
    public interface IFinanceService
    {
        Task AddTransactionAsync(Transaction transaction, CancellationToken ct = default);
        Task DeleteTransactionAsync(Guid id, CancellationToken ct = default);
        Task<decimal> GetBalanceAsync(CancellationToken ct = default);
        Task<IEnumerable<Transaction>> GetFilteredTransactionsAsync(TransactionFilterDto filter, CancellationToken ct = default);
        Task UpdateTransactionAsync(Transaction transaction, CancellationToken ct = default);
    }
}