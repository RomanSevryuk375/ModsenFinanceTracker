using Microsoft.Extensions.Logging;
using Modsen.FinanceTracker.Domain.Extensions;

namespace Modsen.FinanceTracker.BLL.Services;

public sealed class SchedulerService(
    IWalletRepository walletRepository,
    ITransactionFactory factory,
    IUnitOfWork unitOfWork,
    ILogger<SchedulerService> logger) : ISchedulerService
{
    public event EventHandler<TransactionWrittenOffEventArgs>? OnTransactionWrittenOff;

    public async Task<Result> CheckAndProcessRecurringTransactionsAsync(CancellationToken cancellationToken)
    {
        IEnumerable<Wallet> wallets = await walletRepository.GetAllAsync(
            null, null, null, cancellationToken);

        int txCount = 0;
        foreach (Wallet wallet in wallets)
        {
            bool walletModified = false;

            foreach (RecurringTransactionTemplate template in wallet.Templates)
            {
                walletModified = WriteOffTransaction(template, wallet, walletModified);
            }

            if (walletModified)
            {
                await unitOfWork.SaveChangesAsync(cancellationToken);
            }

            txCount++;
        }

        logger.LogInformation("Scheduler processed {Count} wallets and executed {TxCount} transactions.",
            wallets.Count(),
            txCount.ToString());

        return Result.Success();
    }

    private bool WriteOffTransaction(RecurringTransactionTemplate template, Wallet wallet, bool walletModified)
    {
        if (template.NextExecutionDate <= DateTime.Now)
        {
            Transaction transaction = factory.CreateTransaction(
                template.Category.Type,
                template.Amount,
                template.Description,
                template.Category);

            Result addResult = wallet.AddTransaction(transaction);

            if (addResult.IsSuccess)
            {
                template.MoveToNextPeriod();

                walletModified = true;

                OnTransactionWrittenOff?.Invoke(this, new TransactionWrittenOffEventArgs(
                    wallet.Id,
                    wallet.Name,
                    template.Name,
                    template.Amount,
                    template.NextExecutionDate));
            }
        }

        return walletModified;
    }
}


