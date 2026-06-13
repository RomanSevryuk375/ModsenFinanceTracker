using Modsen.FinanceTracker.Domain.Entities;
using Modsen.FinanceTracker.Domain.Interfaces;

namespace Modsen.FinanceTracker.DAL.Repositories;

public sealed class TransactionRepository(IDataContext context) 
    : BaseRepository<Transaction>(context.Transactions), ITransactionRepository 
{
}