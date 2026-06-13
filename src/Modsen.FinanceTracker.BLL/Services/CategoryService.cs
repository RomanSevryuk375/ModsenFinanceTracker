using Modsen.FinanceTracker.BLL.Interfaces;
using Modsen.FinanceTracker.Domain.Entities;
using Modsen.FinanceTracker.Domain.Enums;
using Modsen.FinanceTracker.Domain.Interfaces;

namespace Modsen.FinanceTracker.BLL.Services;

public sealed class CategoryService(IRepository<Category> repository) : ICategoryService
{
    public async Task<IEnumerable<Category>> GetAllCategoriesAsync(
        CancellationToken cancellationToken = default)
    {
        return await repository.GetAllAsync(
            cancellationToken: cancellationToken);
    }

    public async Task<IEnumerable<Category>> GetCategoriesByTypeAsync(
        TransactionType type, 
        CancellationToken cancellationToken = default)
    {
        return await repository.GetAllAsync(
            c => c.Type == type, cancellationToken: cancellationToken);
    }
}