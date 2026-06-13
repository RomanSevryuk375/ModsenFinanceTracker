using Modsen.FinanceTracker.UI.Models;

namespace Modsen.FinanceTracker.UI.Interfaces;

public interface ITransactionListView
{
    void Render(IEnumerable<TransactionRowModel> rows);
}