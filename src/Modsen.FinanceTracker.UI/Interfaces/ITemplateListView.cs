namespace Modsen.FinanceTracker.UI.Interfaces;

public interface ITemplateListView
{
    public void Render(IReadOnlyList<RecurringTransactionTemplate> templates);
}
