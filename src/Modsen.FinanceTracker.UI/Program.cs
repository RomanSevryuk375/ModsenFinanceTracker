using Microsoft.Extensions.DependencyInjection;
using Modsen.FinanceTracker.BLL.Factories;
using Modsen.FinanceTracker.BLL.Interfaces;
using Modsen.FinanceTracker.BLL.Services;
using Modsen.FinanceTracker.BLL.Validators;
using Modsen.FinanceTracker.DAL.Context;
using Modsen.FinanceTracker.DAL.Repositories;
using Modsen.FinanceTracker.Domain.Entities;
using Modsen.FinanceTracker.Domain.Interfaces;
using Modsen.FinanceTracker.Infrastructure.Configuration;
using Modsen.FinanceTracker.UI.Actions;
using Modsen.FinanceTracker.UI.Interfaces;
using Modsen.FinanceTracker.UI.Menu;
using Modsen.FinanceTracker.UI.Views;
using Spectre.Console;

namespace Modsen.FinanceTracker.UI;

class Program
{
    static async Task Main(string[] args)
    {
        using var cts = new CancellationTokenSource();

        Console.CancelKeyPress += (s, e) =>
        {
            e.Cancel = true;
            cts.Cancel();
            Console.WriteLine("\nCancellation requested...");
        };

        try
        {
            var config = AppConfiguration.Instance;
            var services = new ServiceCollection();

            services.AddSingleton<JsonDbContext>(sp => new JsonDbContext(config.JsonDbPath));
            services.AddSingleton<IDataContext>(sp => sp.GetRequiredService<JsonDbContext>());

            services.AddSingleton<ICategoryRepository, JsonCategoryRepository>();
            services.AddSingleton<ITransactionRepository, JsonTransactionRepository>();

            services.AddSingleton<IValidator<Transaction>, TransactionValidator>();
            services.AddSingleton<IFinanceService, FinanceService>();
            services.AddSingleton<ICategoryService, CategoryService>();
            services.AddSingleton<IReportService, ReportService>();

            services.AddTransient<ITransactionFactory, TransactionFactory>();
            services.AddTransient<ITransactionListView, TransactionListView>();
            services.AddTransient<IAnalyticsListView, AnalyticsListView>();

            services.AddTransient<IMenuAction, ViewTransactionAction>();
            services.AddTransient<IMenuAction, CheckBalanceAction>();
            services.AddTransient<IMenuAction, AnalyticsAction>();
            services.AddTransient<IMenuAction, AddTransactionAction>();
            services.AddTransient<IMenuAction, UpdateTransactionAction>();
            services.AddTransient<IMenuAction, DeleteTransactionAction>();
            services.AddTransient<IMenuAction, ExportReportAction>();
            services.AddTransient<IMenuAction, ExitAction>();

            services.AddSingleton<IMainMenu, MainMenu>();
            services.AddSingleton<IApp, App>();

            var serviceProvider = services.BuildServiceProvider();

            var context = serviceProvider.GetRequiredService<JsonDbContext>();
            await context.LoadAsync(cts.Token);

            var categoryRepo = serviceProvider.GetRequiredService<ICategoryRepository>();
            await categoryRepo.SeedAsync(cts.Token);

            var app = serviceProvider.GetRequiredService<IApp>();
            await app.RunAsync(cts.Token);
        }
        catch (OperationCanceledException)
        {
            AnsiConsole.MarkupLine("Application stopped");
        }
        catch (Exception ex)
        {
            AnsiConsole.WriteException(ex);
        }
        finally
        {
            AnsiConsole.MarkupLine("Exiting...");
        }
    }
}