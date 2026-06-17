using Modsen.FinanceTracker.Domain.Extensions;
using Modsen.FinanceTracker.Domain.ValueObjects;

namespace Modsen.FinanceTracker.BLL.Services;

public sealed class FinanceService(
    IWalletRepository repository,
    ICurrencyService currencyService,
    IUnitOfWork unitOfWork,
    string sytemCurrency) : IFinanceService
{
    private readonly string _sytemCurrency = sytemCurrency ?? string.Empty;

    public event EventHandler<CategoryLimitExceededEventArgs>? OnCategoryLimitExceeded;
    public async Task<Result> AddTransactionAsync(
        Guid walletId,
        Transaction transaction,
        CancellationToken cancellationToken = default)
    {
        Wallet? wallet = await repository.GetByIdAsync(walletId, cancellationToken);
        if (wallet is null)
        {
            return Result.Fail($"Wallet {walletId} not found.");
        }

        if (transaction is ExpenseTransaction)
        {
            await CheckBudgetLimitAsync(walletId, transaction, cancellationToken);
        }

        Result addResult = wallet.AddTransaction(transaction);
        if (addResult.IsFailure)
        {
            return Result.Fail(addResult.Error);
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async Task<Result> DeleteTransactionAsync(
        Guid walletId,
        Guid transactionId,
        CancellationToken cancellationToken = default)
    {
        Wallet? wallet = await repository.GetByIdAsync(walletId, cancellationToken);
        if (wallet is null)
        {
            return Result.Fail($"Wallet {walletId} not found.");
        }

        Transaction? transaction = wallet.Transactions.FirstOrDefault(x => x.Id == transactionId);
        if (transaction is null)
        {
            return Result.Fail($"Transaction {transactionId} not found.");
        }

        Result deleteResult = wallet.RemoveTransaction(transaction);
        if (deleteResult.IsFailure)
        {
            return Result.Fail(deleteResult.Error);
        }
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async Task<Result> UpdateTransactionAsync(
        Guid walletId,
        decimal newAmount,
        string newDescription,
        Guid transactionId,
        CancellationToken cancellationToken = default)
    {
        Wallet? wallet = await repository.GetByIdAsync(walletId, cancellationToken);
        if (wallet is null)
        {
            return Result.Fail($"Wallet {walletId} not found.");
        }

        Transaction? transaction = wallet.Transactions.FirstOrDefault(x => x.Id == transactionId);
        if (transaction is null)
        {
            return Result.Fail($"Transaction {transactionId} not found.");
        }

        Result<Money> newAmountResult = Money.Create(newAmount, wallet.BaseCurrency);
        if (newAmountResult.IsFailure)
        {
            return Result.Fail(newAmountResult.Error);
        }

        Result<TransactionDescription> newDescriptionResult = TransactionDescription.Create(newDescription);
        if (newDescriptionResult.IsFailure)
        {
            return Result.Fail(newDescriptionResult.Error);
        }

        Result updateResult = wallet.UpdateTransaction(
            transaction,
            newAmountResult.Value,
            newDescriptionResult.Value);
        if (updateResult.IsFailure)
        {
            return Result.Fail(updateResult.Error);
        }
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async Task<Result<decimal>> GetBalanceAsync(
        Guid walletId,
        CancellationToken cancellationToken = default)
    {
        Wallet? wallet = await repository.GetByIdAsync(walletId, cancellationToken);
        if (wallet is null)
        {
            return Result.Fail<decimal>($"Wallet {walletId} not found.");
        }

        Result<decimal> exchangeRateResult = await currencyService.GetExchangeRateAsync(
            wallet.BaseCurrency, _sytemCurrency, cancellationToken);
        if (exchangeRateResult.IsFailure)
        {
            return Result.Fail<decimal>(exchangeRateResult.Error);
        }

        decimal actualBalance = wallet.Balance.Amount * exchangeRateResult.Value;

        return Result.Success(actualBalance);
    }

    public async Task<Result<IReadOnlyList<Transaction>>> GetFilteredTransactionsAsync(
        Guid walletId,
        TransactionFilterDto filter,
        CancellationToken cancellationToken = default)
    {
        Wallet? wallet = await repository.GetByIdAsync(walletId, cancellationToken);
        if (wallet is null)
        {
            return Result.Fail<IReadOnlyList<Transaction>>($"Wallet {walletId} not found.");
        }

        Func<Transaction, bool> isMatch = filter.ToFilter();

        var filteredList = wallet.Transactions
            .Where(isMatch)
            .ToList();

        return Result.Success<IReadOnlyList<Transaction>>(filteredList);
    }

    private async Task CheckBudgetLimitAsync(
        Guid walletId,
        Transaction transaction,
        CancellationToken cancellationToken = default)
    {
        var filter = new TransactionFilterDto
        {
            SearchTerm = null,
            From = DateTime.Now.AddMonths(-1),
            To = DateTime.Now
        };

        Result<IReadOnlyList<Transaction>> filterResult = await GetFilteredTransactionsAsync(
            walletId, filter, cancellationToken);
        if (filterResult.IsFailure)
        {
            return;
        }

        decimal transactionsAmount = filterResult.Value.ToList()
            .Where(x => x.Category.Id == transaction.Category.Id)
            .Sum(x => x.Amount.Amount);

        if (transaction.Category.BudgetLimit.HasValue &&
            (transactionsAmount + transaction.Amount.Amount) > transaction.Category.BudgetLimit.Value)
        {
            OnCategoryLimitExceeded?.Invoke(this, new CategoryLimitExceededEventArgs
            {
                CategoryName = transaction.Category.Name,
                ExcessAmount = transactionsAmount + transaction.Amount.Amount - transaction.Category.BudgetLimit.Value
            });
        }
    }

    public async Task<Result> AddTemplateAsync(
        Guid walletId,
        RecurringTransactionTemplate template,
        CancellationToken cancellationToken = default)
    {
        Wallet? wallet = await repository.GetByIdAsync(walletId, cancellationToken);
        if (wallet is null)
        {
            return Result.Fail($"Wallet {walletId} not found.");
        }

        Result addResult = wallet.AddTransactionTemplate(template);
        if (addResult.IsFailure)
        {
            return Result.Fail(addResult.Error);
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async Task<Result> DeleteTemplateAsync(
        Guid walletId,
        Guid templateId,
        CancellationToken cancellationToken = default)
    {
        Wallet? wallet = await repository.GetByIdAsync(walletId, cancellationToken);
        if (wallet is null)
        {
            return Result.Fail($"Wallet {walletId} not found.");
        }

        RecurringTransactionTemplate? template = wallet.Templates.FirstOrDefault(t => t.Id == templateId);
        if (template is null)
        {
            return Result.Fail("Template not found in this wallet.");
        }

        Result deleteResult = wallet.RemoveTransactionTemplate(template);
        if (deleteResult.IsFailure)
        {
            return Result.Fail(deleteResult.Error);
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
