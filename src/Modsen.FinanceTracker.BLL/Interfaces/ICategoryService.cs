using Modsen.FinanceTracker.Domain.Entities;

namespace Modsen.FinanceTracker.BLL.Interfaces
{
    public interface ICategoryService
    {
        Task<IEnumerable<Category>> GetAllCategoriesAsync(CancellationToken ct = default);
    }
}