using Modsen.FinanceTracker.DAL.Context;
using Modsen.FinanceTracker.Domain.Entities;
using Modsen.FinanceTracker.Domain.Interfaces;

namespace Modsen.FinanceTracker.DAL.Repositories;

public class TransactionRepository : BaseRepository<Transaction>, ITransactionRepository 
{
    public TransactionRepository(IDataContext context) : base(context.Transactions)
    {
    }
}