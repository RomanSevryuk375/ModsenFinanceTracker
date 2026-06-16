using Modsen.FinanceTracker.Domain.ValueObjects;

namespace Modsen.FinanceTracker.Domain.Events;

public sealed record TransactionWrittenOffEventArgs(
    Guid WalletId,
    string WalletName,
    string Description,
    Money Amount,
    DateTime NextExecutionDate);
