using Modsen.FinanceTracker.BLL.DTOs;
using Modsen.FinanceTracker.BLL.Interfaces;
using Modsen.FinanceTracker.Domain.Entities;
using Modsen.FinanceTracker.Domain.Events;
using Modsen.FinanceTracker.Domain.Interfaces;

namespace Modsen.FinanceTracker.BLL.Services;

public sealed class FinanceService(
    ITransactionRepository repository,
    ICategoryRepository categoryRepository,
    IValidator<Transaction> validator) : IFinanceService
{
    public event EventHandler<CategoryLimitExceededEventArgs>? OnCategoryLimitExceeded;
    public async Task AddTransactionAsync(
        Transaction transaction,
        CancellationToken cancellationToken = default)
    {
        var (isValid, message) = validator.Validate(transaction);
        if (!isValid)
        {
            throw new ArgumentException(message);
        }

        if (transaction is ExpenseTransaction)
        {
            await CheckBudgetLimitAsync(transaction, cancellationToken);
        }

        await repository.AddAsync(transaction, cancellationToken);
    }

    public async Task DeleteTransactionAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        await repository.DeleteAsync(id, cancellationToken);
    }

    public async Task UpdateTransactionAsync(
        Transaction transaction,
        CancellationToken cancellationToken = default)
    {
        await repository.UpdateAsync(transaction, cancellationToken);
    }

    public async Task<decimal> GetBalanceAsync(
        CancellationToken cancellationToken = default)
    {
        var transactions = await repository.GetAllAsync(
            cancellationToken: cancellationToken);

        return transactions.Sum(t =>
            t is IncomeTransaction
                ? t.Amount
                : -t.Amount);
    }

    public async Task<IEnumerable<Transaction>> GetFilteredTransactionsAsync(
        TransactionFilterDto filter,
        CancellationToken cancellationToken = default)
    {
        return await repository.GetAllAsync(
            filter.ToExpression(), cancellationToken: cancellationToken);
    }

    private async Task CheckBudgetLimitAsync(Transaction transaction, CancellationToken cancellationToken)
    {
        decimal transactionsAmount = 0m;

        var filter = new TransactionFilterDto
        {
            SearchTerm = null,
            From = DateTime.Now.AddMonths(-1),
            To = DateTime.Now
        };

        transactionsAmount = (await GetFilteredTransactionsAsync(filter, cancellationToken))
            .ToList()
            .Where(x => x.CategoryId == transaction.CategoryId)
            .Sum(x => x.Amount);

        var category = await categoryRepository.GetByIdAsync(transaction.CategoryId, cancellationToken);
        if (category is null)
        {
            throw new ArgumentException($"Category {transaction.CategoryId} not found");
        }

        if (category.BudgetLimit.HasValue &&
            (transactionsAmount + transaction.Amount) > category.BudgetLimit.Value)
        {
            OnCategoryLimitExceeded?.Invoke(this, new CategoryLimitExceededEventArgs 
            { 
                CategoryName = category.Name, 
                ExcessAmount = transactionsAmount + transaction.Amount - category.BudgetLimit.Value
            });
        }
    }
}