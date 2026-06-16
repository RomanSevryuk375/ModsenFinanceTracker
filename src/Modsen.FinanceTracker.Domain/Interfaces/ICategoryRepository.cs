namespace Modsen.FinanceTracker.Domain.Interfaces;

public interface ICategoryRepository : IRepository<Category>
{
    public Task SeedAsync(CancellationToken cancellationToken = default);
}