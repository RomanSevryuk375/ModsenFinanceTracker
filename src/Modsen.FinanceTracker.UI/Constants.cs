namespace Modsen.FinanceTracker.UI;

public class Constants
{
    public static class MainMenu
    {
        public const string MenuHeader = "[yellow]Main Menu[/]";
        public const string SelectOptionPrompt = "Please select an [green]option[/]:";
        public const int MainPageSize = 10;
        
        public const string ActionAdd = "Add Transaction";
        public const string ActionView = "View History";
        public const string ActionDelete = "Delete Transaction";
        public const string ActionBalance = "Check Balance";
        public const string ActionExit = "Exit";

        public static readonly string[] AllActions = 
        { 
            ActionAdd, ActionView, ActionDelete, ActionBalance, ActionExit 
        };
    }

    public static class Colors
    {
        public const string Success = "green";
        public const string Error = "red";
        public const string Info = "blue";
        public const string Wait = "grey";
    }
}