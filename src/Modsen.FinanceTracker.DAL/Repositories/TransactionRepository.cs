using Modsen.FinanceTracker.DAL.Context;
using Modsen.FinanceTracker.Domain.Entities;
using Modsen.FinanceTracker.Domain.Interfaces;

namespace Modsen.FinanceTracker.DAL.Repositories;

public class TransactionRepository : IRepository<Transaction>
{
    public IEnumerable<Transaction> GetAll() => InMemoryStorage.Transactions;
    
    public void Add(Transaction entity) => InMemoryStorage.Transactions.Add(entity);

    public void Delete(Guid id)
    {
        var transaction = InMemoryStorage.Transactions.FirstOrDefault(t => t.Id == id);
        if (transaction != null)
        {
            InMemoryStorage.Transactions.Remove(transaction);
        }
    }
}