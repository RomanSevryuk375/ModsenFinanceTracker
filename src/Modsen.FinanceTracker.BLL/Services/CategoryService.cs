using Modsen.FinanceTracker.Domain.Extensions;

namespace Modsen.FinanceTracker.BLL.Services;

public sealed class CategoryService(ICategoryRepository repository) : ICategoryService
{
    public async Task<Result<IEnumerable<Category>>> GetAllCategoriesAsync(
        CancellationToken cancellationToken = default)
    {
        IEnumerable<Category> categories = await repository.GetAllAsync(
            cancellationToken: cancellationToken);
        return Result.Success(categories);
    }

    public async Task<Result<IEnumerable<Category>>> GetCategoriesByTypeAsync(
        TransactionType type, CancellationToken cancellationToken = default)
    {
        IEnumerable<Category> categories = await repository.GetAllAsync(
            c => c.Type == type, cancellationToken: cancellationToken);
        return Result.Success(categories);
    }
}
