using Modsen.FinanceTracker.Domain.Entities;

namespace Modsen.FinanceTracker.Domain.Interfaces;

public interface ITransactionRepository
{
    Task AddAsync(Transaction entity);
    Task UpdateAsync(Transaction entity);
    Task DeleteAsync(Guid id);
}