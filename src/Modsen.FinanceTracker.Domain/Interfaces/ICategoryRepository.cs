using Modsen.FinanceTracker.Domain.Entities;

namespace Modsen.FinanceTracker.Domain.Interfaces;

public interface ICategoryRepository : IRepository<Category>
{
    public Task SeedAsync(CancellationToken cancellationToken = default);
}