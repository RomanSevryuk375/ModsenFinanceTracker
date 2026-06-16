using Modsen.FinanceTracker.Domain.ValueObjects;

namespace Modsen.FinanceTracker.BLL.Interfaces;

public interface ITransactionFactory
{
    public Transaction? CreateTransaction(
        TransactionType type,
        Money amount,
        TransactionDescription description,
        Category category);
}
