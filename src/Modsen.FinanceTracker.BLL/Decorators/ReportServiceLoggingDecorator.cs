using Microsoft.Extensions.Logging;
using Modsen.FinanceTracker.Domain.Extensions;

namespace Modsen.FinanceTracker.BLL.Decorators;

public sealed class ReportServiceLoggingDecorator(
    IReportService inner,
    ILogger<ReportServiceLoggingDecorator> logger) : IReportService
{
    public async Task<Result> ExportAsync(
        Guid walletId,
        IExportStrategy strategy,
        string filePath, CancellationToken cancellationToken)
    {
        logger.LogInformation("Starting export for wallet {WalletId} using {StrategyType}",
            walletId, strategy.GetType().Name);

        try
        {
            Result result = await inner.ExportAsync(walletId, strategy, filePath, cancellationToken);

            if (result.IsFailure)
            {
                logger.LogWarning("Export failed: {Error}", result.Error);
            }
            else
            {
                logger.LogInformation("Export completed successfully to {FilePath}", filePath);
            }

            return result;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "System exception during export to {FilePath}", filePath);
            return Result.Fail("An unexpected error occurred during export.");
        }
    }
}
