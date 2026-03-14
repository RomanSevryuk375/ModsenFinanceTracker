using Modsen.FinanceTracker.Domain.Entities;

namespace Modsen.FinanceTracker.Domain.Interfaces;

public interface IDataContext
{ 
    List<Transaction> Transactions { get; }
    List<Category> Categories { get; }
}