using Modsen.FinanceTracker.DAL.Context;
using Modsen.FinanceTracker.Domain.Entities;
using Modsen.FinanceTracker.Domain.Enums;
using Modsen.FinanceTracker.Domain.Interfaces;

namespace Modsen.FinanceTracker.DAL.Repositories;

public sealed class JsonCategoryRepository(JsonDbContext context)
    : BaseRepository<Category>(context.Categories), ICategoryRepository
{

    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        if (context.Categories.Count == 0)
        {
            await AddAsync(new Category(Guid.NewGuid(), "Salary", TransactionType.Income), cancellationToken);
            await AddAsync(new Category(Guid.NewGuid(), "Food", TransactionType.Expense), cancellationToken);

            await context.SaveChangesAsync(cancellationToken);
        }
    }
}