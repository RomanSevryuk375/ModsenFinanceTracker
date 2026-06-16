using Microsoft.Extensions.Logging;
using Modsen.FinanceTracker.Domain.Extensions;

namespace Modsen.FinanceTracker.BLL.Decorators;

public sealed class FinanceServiceLoggingDecorator : IFinanceService
{
    private readonly IFinanceService _inner;
    private readonly ILogger<FinanceServiceLoggingDecorator> _logger;

    public event EventHandler<CategoryLimitExceededEventArgs>? OnCategoryLimitExceeded;

    public FinanceServiceLoggingDecorator(IFinanceService inner, ILogger<FinanceServiceLoggingDecorator> logger)
    {
        _inner = inner;
        _logger = logger;

        _inner.OnCategoryLimitExceeded += HandleCategoryLimitExceeded;
    }

    private void HandleCategoryLimitExceeded(object? sender, CategoryLimitExceededEventArgs args)
    {
        _logger.LogWarning(
            "BUDGET ALERT: Category '{CategoryName}' exceeded by {ExcessAmount}",
            args.CategoryName,
            args.ExcessAmount);

        OnCategoryLimitExceeded?.Invoke(this, args);
    }

    public async Task<Result> AddTransactionAsync(
        Guid walletId,
        Transaction transaction,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Attempting to add transaction to wallet {WalletId}", walletId);

        Result result = await _inner.AddTransactionAsync(walletId, transaction, cancellationToken);
        LogResult(result, nameof(AddTransactionAsync));

        return result;
    }

    public async Task<Result> DeleteTransactionAsync(
        Guid walletId,
        Guid transactionId,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Attempting to delete transaction {TransactionId} from wallet {WalletId}",
            transactionId, walletId);

        Result result = await _inner.DeleteTransactionAsync(walletId, transactionId, cancellationToken);
        LogResult(result, nameof(DeleteTransactionAsync));

        return result;
    }

    public async Task<Result> UpdateTransactionAsync(
        Guid walletId,
        decimal newAmount,
        string newDescription,
        Guid transactionId,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Attempting to update transaction {TransactionId} in wallet {WalletId}",
            transactionId, walletId);

        Result result = await _inner.UpdateTransactionAsync(
            walletId, newAmount, newDescription, transactionId, cancellationToken);
        LogResult(result, nameof(UpdateTransactionAsync));

        return result;
    }

    public async Task<Result<decimal>> GetBalanceAsync(
        Guid walletId,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Fetching balance for wallet {WalletId}", walletId);

        return await _inner.GetBalanceAsync(walletId, cancellationToken);
    }

    public async Task<Result<IReadOnlyList<Transaction>>> GetFilteredTransactionsAsync(
        Guid walletId,
        TransactionFilterDto filter,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Fetching filtered transactions for wallet {WalletId}", walletId);

        return await _inner.GetFilteredTransactionsAsync(walletId, filter, cancellationToken);
    }

    public async Task<Result> AddTemplateAsync(
        Guid walletId,
        RecurringTransactionTemplate template,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Attempting to add template to wallet {WalletId}", walletId);

        Result result = await _inner.AddTemplateAsync(walletId, template, cancellationToken);
        LogResult(result, nameof(AddTemplateAsync));

        return result;
    }

    public async Task<Result> DeleteTemplateAsync(
        Guid walletId,
        Guid templateId,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Attempting to delete template {TemplateId} from wallet {WalletId}",
            templateId, walletId);

        Result result = await _inner.DeleteTemplateAsync(walletId, templateId, cancellationToken);
        LogResult(result, nameof(DeleteTemplateAsync));

        return result;
    }

    private void LogResult(Result result, string operation)
    {
        if (result.IsFailure)
        {
            _logger.LogWarning("{Operation} failed with error: {Error}", operation, result.Error);
        }
        else
        {
            _logger.LogInformation("{Operation} completed successfully.", operation);
        }
    }
}
