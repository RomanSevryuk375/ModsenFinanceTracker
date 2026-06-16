using Modsen.FinanceTracker.Domain.Extensions;

namespace Modsen.FinanceTracker.BLL.Interfaces;

public interface ICategoryService
{
    public Task<Result<IEnumerable<Category>>> GetAllCategoriesAsync(
        CancellationToken cancellationToken = default);

    public Task<Result<IEnumerable<Category>>> GetCategoriesByTypeAsync(
        TransactionType type,
        CancellationToken cancellationToken = default);
}
