namespace Modsen.FinanceTracker.UI;

public class Constants
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
    }

    public static class Tables
    {
        public const string TransactionsTableTitle = "List of Transactions";
        public const string TransactionsTableId = "Id";
        public const string TransactionsDate = "Date";
        public const string TransactionsAmount = "Amount";
        public const string TransactionDescription = "Description";
        public const string TransactionCategory = "Category";
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