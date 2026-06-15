using Modsen.FinanceTracker.Domain.Events;

namespace Modsen.FinanceTracker.BLL.Interfaces;

public interface ISchedulerService
{
    public event EventHandler<TransactionWrittenOffEventArgs>? OnTransactionWrittenOff;
    public Task CheckAndProcessRecurringTransactionsAsync(CancellationToken cancellationToken);
}
