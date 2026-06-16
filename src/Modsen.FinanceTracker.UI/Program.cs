using Microsoft.Extensions.DependencyInjection;
using Modsen.FinanceTracker.BLL.Extensions;
using Modsen.FinanceTracker.DAL.Extensions;
using Modsen.FinanceTracker.Infrastructure.Configuration;
using Modsen.FinanceTracker.Infrastructure.Extensions;
using Modsen.FinanceTracker.UI.Extensions;
using Serilog;

namespace Modsen.FinanceTracker.UI;

internal class Program
{
    private static async Task Main()
    {
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .WriteTo.File("app.log", rollingInterval: RollingInterval.Day)
            .CreateLogger();

        using var cts = new CancellationTokenSource();

        Console.CancelKeyPress += (s, e) =>
        {
            e.Cancel = true;
            cts.Cancel();
            Console.WriteLine("\nCancellation requested...");
        };

        try
        {
            Log.Information("Application is starting up...");

            AppConfiguration config = AppConfiguration.Instance;
            var services = new ServiceCollection();

            services.AddInfrastructure()
                    .AddBLL(config.Currency)
                    .AddDAL(config.JsonDbPath)
                    .AddUi(config.Currency)
                    .AddLogging(builder => builder.AddSerilog(dispose: true)); ;

            IServiceProvider serviceProvider = services.BuildServiceProvider();
            await serviceProvider.InitApplicationAsync(cts.Token);
        }
        catch (OperationCanceledException)
        {
            Log.Information("Application stopped by user (Ctrl+C).");
            AnsiConsole.MarkupLine("Application stopped");
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Application terminated unexpectedly!");
            AnsiConsole.WriteException(ex);
        }
        finally
        {
            Log.Information("Application shut down.");
            Log.CloseAndFlush(); 
            AnsiConsole.MarkupLine("Exiting...");
        }
    }
}
