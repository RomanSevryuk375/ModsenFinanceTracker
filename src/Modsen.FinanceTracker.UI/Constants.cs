namespace Modsen.FinanceTracker.UI;

public static class Constants
{
    public static class MainMenu
    {
        public const string MenuHeader = "[yellow]Modsen Finance Tracker[/]";
        public const string SelectOptionPrompt = "Please select an [green]option[/]:";
        public const int MainPageSize = 10;

        public const string ActionAdd = "Add Transaction";
        public const string ActionUpdate = "Update Transaction";
        public const string ActionView = "View History";
        public const string ActionDelete = "Delete Transaction";
        public const string ActionBalance = "Check Balance";
        public const string ActionExit = "Exit";
        public const string ActionExportReport = "Export Reports";
        public const string ActionAnalytics = "Analytics";
    }

    public static class Tables
    {
        public const string Title = "List of Transactions";
        public const string AnalyticsTitle = "Analytics of Transactions";
        public const string Id = "Id";
        public const string Date = "Date";
        public const string Amount = "Amount";
        public const string Description = "Description";
        public const string Category = "Category";
        public const string Percent = "%";
    }

    public static class BreakdownChart
    {
        public const string Name = "Expenses Breakdown";
        public const int Width = 60;
        public const int PaddingTop = 1;
        public const int PaddingLeft = 2;
        public const int PaddingRight = 2;
        public const int PaddingBottom = 1;

    }

    public static class Colors
    {
        public const string Primary = "yellow";
        public const string Success = "green";
        public const string Error = "red";
        public const string Info = "blue";
    }

    public static class Balance
    {
        public const string Header = "Summary";
        public const string MessageTemplate = "Your current balance is: [{0}]{1:N2} {2}[/]";
    }

    public static class Layout
    {
        public const int PanelPaddingHorizontal = 2;
        public const int PanelPaddingVertical = 1;
    }
}