using Modsen.FinanceTracker.BLL.DTOs;
using Modsen.FinanceTracker.Domain;
using Modsen.FinanceTracker.Domain.Entities;
using Modsen.FinanceTracker.Domain.Events;

namespace Modsen.FinanceTracker.BLL.Interfaces;

public interface IFinanceService
{
    public event EventHandler<CategoryLimitExceededEventArgs>? OnCategoryLimitExceeded;

    public Task<Result> AddTransactionAsync(
        Guid walletId, 
        Transaction transaction, 
        CancellationToken cancellationToken = default);

    public Task<Result> DeleteTransactionAsync(
        Guid walletId, 
        Guid transactionId, 
        CancellationToken cancellationToken = default);

    public Task<Result<decimal>> GetBalanceAsync(
        Guid walletId, 
        CancellationToken cancellationToken = default);

    public Task<Result<IReadOnlyList<Transaction>>> GetFilteredTransactionsAsync(
        Guid walletId, 
        TransactionFilterDto filter, 
        CancellationToken cancellationToken = default);

    public Task<Result> UpdateTransactionAsync(
        Guid walletId, 
        decimal newAmount, 
        string newDescription, 
        Guid transactionId, 
        CancellationToken cancellationToken = default);

    public Task<Result> AddTemplateAsync(
        Guid walletId,
        RecurringTransactionTemplate template,
        CancellationToken cancellationToken = default);

    public Task<Result> DeleteTemplateAsync(
        Guid walletId,
        Guid templateId,
        CancellationToken cancellationToken = default);
}
