using Modsen.FinanceTracker.Domain.Entities;
using Modsen.FinanceTracker.Domain.Enums;

namespace Modsen.FinanceTracker.BLL.Interfaces;

public interface ITransactionFactory
{
    Transaction CreateTransaction(
        TransactionType type,
        decimal amount,
        string description,
        Guid categoryId);
}