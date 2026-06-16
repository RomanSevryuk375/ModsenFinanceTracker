using Microsoft.Extensions.DependencyInjection;
using Modsen.FinanceTracker.BLL.Extensions;
using Modsen.FinanceTracker.DAL.Extensions;
using Modsen.FinanceTracker.Infrastructure.Configuration;
using Modsen.FinanceTracker.Infrastructure.Extensions;
using Modsen.FinanceTracker.UI.Extensions;
using Spectre.Console;

namespace Modsen.FinanceTracker.UI;

internal class Program
{
    private static async Task Main()
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
            AppConfiguration config = AppConfiguration.Instance;
            var services = new ServiceCollection();

            services.AddInfrastructure()
                    .AddBLL(config.Currency)
                    .AddDAL(config.JsonDbPath)
                    .AddUi(config.Currency);

            IServiceProvider serviceProvider = services.BuildServiceProvider();
            await serviceProvider.InitApplicationAsync(cts.Token);
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
