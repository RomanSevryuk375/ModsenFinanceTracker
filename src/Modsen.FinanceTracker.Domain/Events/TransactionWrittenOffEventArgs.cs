namespace Modsen.FinanceTracker.Domain.Events;

public sealed record TransactionWrittenOffEventArgs(
    Guid WalletId,
    string WalletName,
    string Description,
    decimal Amount,
    DateTime NextExecutionDate);
