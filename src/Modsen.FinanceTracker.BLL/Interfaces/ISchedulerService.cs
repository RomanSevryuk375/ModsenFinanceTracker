using Modsen.FinanceTracker.Domain.Extensions;

namespace Modsen.FinanceTracker.BLL.Interfaces;

public interface ISchedulerService
{
    public event EventHandler<TransactionWrittenOffEventArgs>? OnTransactionWrittenOff;
    public Task<Result> CheckAndProcessRecurringTransactionsAsync(
        CancellationToken cancellationToken);
}
