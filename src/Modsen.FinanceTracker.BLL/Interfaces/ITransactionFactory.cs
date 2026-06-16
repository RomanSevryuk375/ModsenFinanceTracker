namespace Modsen.FinanceTracker.BLL.Interfaces;

public interface ITransactionFactory
{
    public Transaction CreateTransaction(
        TransactionType type,
        decimal amount,
        string description,
        Category category);
}