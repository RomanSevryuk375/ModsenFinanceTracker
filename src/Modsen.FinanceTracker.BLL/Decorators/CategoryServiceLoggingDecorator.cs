using Microsoft.Extensions.Logging;
using Modsen.FinanceTracker.Domain.Extensions;

namespace Modsen.FinanceTracker.BLL.Decorators;

public sealed class CategoryServiceLoggingDecorator(
    ICategoryService inner,
    ILogger<CategoryServiceLoggingDecorator> logger) : ICategoryService
{
    public async Task<Result<IEnumerable<Category>>> GetAllCategoriesAsync(
        CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Fetching all categories.");

        return await inner.GetAllCategoriesAsync(cancellationToken);
    }

    public async Task<Result<IEnumerable<Category>>> GetCategoriesByTypeAsync(
        TransactionType type,
        CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Fetching categories of type {Type}.", type);

        return await inner.GetCategoriesByTypeAsync(type, cancellationToken);
    }
}
