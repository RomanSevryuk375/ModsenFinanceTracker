namespace Modsen.FinanceTracker.UI;

public static class Constants
{
    public static class MainMenu
    {
        public const string MenuHeader = "[yellow]Modsen Finance Tracker[/]";
        public const string SelectOptionPrompt = "Please select an [green]option[/]:";
        public const int MainPageSize = 10;

        public const string ActionBalance = "Check Balance";
        public const string ActionExit = "Exit";
        public const string ActionExportReport = "Export Reports";
        public const string ActionAnalytics = "Analytics";

        public const string ManageTransactions = "Manage Transactions";
        public const string AddTransaction = "Add Transaction";
        public const string UpdateTransaction = "Update Transaction";
        public const string ViewTransaction = "View History";
        public const string DeleteTransaction = "Delete Transaction";

        public const string ManageWallets = "Manage Wallets";
        public const string CreateWallet = "Create Wallet";
        public const string ViewWallets = "View Wallets";
        public const string DeleteWallet = "Delete Wallet";

        public const string BackToMainMenu = "Back to Main Menu";
    }

    public static class Tables
    {
        public const string Title = "List of Transactions";
        public const string WalletsTitle = "List of Wallets";
        public const string AnalyticsTitle = "Analytics of Transactions";

        public const string Id = "Id";
        public const string Date = "Date";
        public const string Amount = "Amount";
        public const string Description = "Description";
        public const string Category = "Category";
        public const string Percent = "%";

        public const string WalletName = "Name";
        public const string WalletCurrency = "Currency";
        public const string WalletBalance = "Current Balance";

        public const int PageSize = 5;
        public const string NavigationTitle = "Navigation: ";
        public const string NextPageButton = "Next Page";
        public const string PrevPageButton = "Previous Page";
        public const string ExitButton = "Exit to Menu";
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

    public static class Success
    {
        public const string AddTransaction = "Transaction added successfully";
        public const string DeleteTransaction = "Transaction deleted successfully";
        public const string UpdateTransaction = "Transaction updated successfully";

        public const string CreateWallet = "Wallet created successfully";
        public const string DeleteWallet = "Wallet deleted successfully";

        public const string ExportReport = "Report exported successfully to:";
    }

    public static class Prompts
    {
        public const string TransactionManagementTitle = "Transaction Management: ";
        public const string WalletManagementTitle = "Wallet Management: ";
        public const string WalletSelectionTitle = $"Select a [{Colors.Success}]wallet[/]:";

        public const string TransactionConfirmation = "Are you sure you want to delete this transaction?";
        public const string SearchConfirmation = "Do you want to search by description?";

        public const string Amount = "Enter amount:";
        public static string NewAmount(decimal targetAmount) => $"New amount (current: {targetAmount}):";
        public const string Category = "Select category:";
        public const string Description = "Enter description:";
        public static string NewDescription(string targetDescription) => $"New description (current: {targetDescription}):";
        public const string DeleteTransaction = "Choose transaction to delete:";
        public const string EditTransaction = "Choose transaction to edit:";
        public const string TransactionType = $"Is this an [{Colors.Info}]Income[/]?";
        public const string SearchTerm = "Enter search term:";
        public const string StartDate = "Start date:";
        public const string EndDate = "End date:";
        public const string ExportFormat = "Select export format:";

        public const string WalletName = "Enter wallet name (e.g., Main, Savings):";
        public const string WalletCurrency = "Enter base currency (3 letters, e.g., USD, EUR):";
        public const string DeleteWalletSelection = "Select a wallet to delete:";

        public static string DeleteWalletConfirmation(string walletName) =>
            $"Are you sure you want to delete [{Colors.Error}]{walletName}[/]? All transactions inside will be lost!";

        public static string WalletDisplay(string name, string currency, decimal balance) =>
            $"{name} ({currency}) - Balance: {balance:N2}";
    }

    public static class Info
    {
        public const string NoWalletsToDelete = $"[{Colors.Info}]No wallets found to delete.[/]";
        public const string NoWalletsToView = $"[{Colors.Info}]No wallets found. Please create one first.[/]";
        public const string DeletionCancelled = $"[{Colors.Info}]Deletion cancelled.[/]";
    }

    public static class Errors
    {
        public const string NegativeAmount = $"[{Colors.Error}]Amount must be positive.[/]";
        public const string BalanceBecomeNegative = $"[{Colors.Error}]Insufficient funds! Current balance will become negative[/]";
        public const string EndInFuture = "End date must be after start date";

        public const string CategoriesNotFound = $"[{Colors.Error}]No categories found.[/]";
        public const string TransactionNotFound = $"[{Colors.Info}]No transactions found.[/]";
        public const string WalletNotFound = $"[{Colors.Info}]No wallets found. Please create a wallet first.[/]";

        public const string InvalidFormat = $"[{Colors.Error}]Invalid format[/]";
        public const string CurrencyLength = $"[{Colors.Error}]Currency must be exactly 3 characters.[/]";
    }

    public static class UI
    {
        public const int GuidShortLength = 8;
        public const int CurrencyLength = 3;
    }
}