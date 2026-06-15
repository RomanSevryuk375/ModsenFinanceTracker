using Modsen.FinanceTracker.Domain.Entities;
using Modsen.FinanceTracker.Domain.Enums;

namespace Modsen.FinanceTracker.BLL.Interfaces;

public interface ICategoryService
{
    public Task<IEnumerable<Category>> GetAllCategoriesAsync(
        CancellationToken cancellationToken = default);

    public Task<IEnumerable<Category>> GetCategoriesByTypeAsync(
        TransactionType type, 
        CancellationToken cancellationToken = default);
}