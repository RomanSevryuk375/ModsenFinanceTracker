using Modsen.FinanceTracker.UI.Models;

namespace Modsen.FinanceTracker.UI.Interfaces;

public interface IAnalyticsListView
{
    void Render(IEnumerable<AnalyticsRowModel> rows);
}