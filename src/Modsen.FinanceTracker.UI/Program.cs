using Modsen.FinanceTracker.BLL.Services;
using Modsen.FinanceTracker.DAL.Context;
using Modsen.FinanceTracker.DAL.Repositories;
using Modsen.FinanceTracker.Infrastructure.Configuration;
using Modsen.FinanceTracker.UI.Actions;
using Modsen.FinanceTracker.UI.Interfaces;
using Modsen.FinanceTracker.UI.Menu;

namespace Modsen.FinanceTracker.UI;

class Program
{
    static async Task Main(string[] args)
    {
        var config = AppConfiguration.Instance;
        
        var context = new JsonDbContext(config.JsonDbPath);
        await context.LoadAsync();
        
        var categoryRepo = new JsonCategoryRepository(context);
        await categoryRepo.SeedAsync(); 

        var transactionRepo = new JsonTransactionRepository(context);
        
        var financeService = new FinanceService(transactionRepo);
        
        var actions = new List<IMenuAction>
        {
            new AddTransactionAction(),
            new CheckBalanceAction(),
            new DeleteTransectionAction(),
            new ViewTransactionAction(),
            new ExitAction(),
        };

        IMainMenu menu = new MainMenu();
        
        IApp app = new App(menu, actions);
        await app.RunAsync(); 
    }
}