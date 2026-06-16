using Microsoft.Extensions.Logging;
using Modsen.FinanceTracker.Domain.Extensions;

namespace Modsen.FinanceTracker.BLL.Decorators;

public sealed class SchedulerServiceLoggingDecorator : ISchedulerService
{
    private readonly ISchedulerService _inner;
    private readonly ILogger<SchedulerServiceLoggingDecorator> _logger;

    public event EventHandler<TransactionWrittenOffEventArgs>? OnTransactionWrittenOff;

    public SchedulerServiceLoggingDecorator(ISchedulerService inner, ILogger<SchedulerServiceLoggingDecorator> logger)
    {
        _inner = inner;
        _logger = logger;

        _inner.OnTransactionWrittenOff += HandleTransactionWrittenOff;
    }

    private void HandleTransactionWrittenOff(object? sender, TransactionWrittenOffEventArgs args)
    {
        _logger.LogInformation(
            "AUTO-PAYMENT: {Amount} deducted from wallet '{WalletName}' for '{Description}'. Next due: {NextDate:d}",
            args.Amount,
            args.WalletName,
            args.Description,
            args.NextExecutionDate);

        OnTransactionWrittenOff?.Invoke(this, args);
    }

    public async Task<Result> CheckAndProcessRecurringTransactionsAsync(
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Scheduler started checking recurring transactions.");

        try
        {
            Result result = await _inner.CheckAndProcessRecurringTransactionsAsync(cancellationToken);
            _logger.LogInformation("Scheduler check completed.");
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Critical error during Scheduler execution.");
            return Result.Fail("Scheduler failed due to system error.");
        }
    }
}
