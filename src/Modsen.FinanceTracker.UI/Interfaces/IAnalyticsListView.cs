using Modsen.FinanceTracker.UI.Models;

namespace Modsen.FinanceTracker.UI.Interfaces;

public interface IAnalyticsListView
{
    public void Render(IEnumerable<AnalyticsRowModel> rows);
}