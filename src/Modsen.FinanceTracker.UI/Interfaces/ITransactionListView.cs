namespace Modsen.FinanceTracker.UI.Interfaces;

public interface ITransactionListView
{
    public void Render(IEnumerable<TransactionRowModel> rows);
}