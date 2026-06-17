using Microsoft.Extensions.Logging;
using Modsen.FinanceTracker.Domain.Extensions;

public sealed class SchedulerServiceLoggingDecorator(
    ISchedulerService inner,
    ILogger<SchedulerServiceLoggingDecorator> logger) : ISchedulerService
{
    public event EventHandler<TransactionWrittenOffEventArgs>? OnTransactionWrittenOff
    {
        add => inner.OnTransactionWrittenOff += value;
        remove => inner.OnTransactionWrittenOff -= value;
    }

    public async Task<Result<int>> CheckAndProcessRecurringTransactionsAsync(
        CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Scheduler started checking recurring transactions.");

        try
        {
            Result<int> result = await inner.CheckAndProcessRecurringTransactionsAsync(cancellationToken);

            if (result.IsSuccess)
            {
                logger.LogInformation("Scheduler check completed. Executed transactions: {Count}", result.Value);
            }

            return result;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Critical error during Scheduler execution.");
            return Result.Fail<int>("Scheduler failed due to system error.");
        }
    }
}
