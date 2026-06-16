namespace Modsen.FinanceTracker.DAL.Repositories;

public sealed class JsonCategoryRepository(
    JsonDbContext context,
    ILogger<JsonCategoryRepository> logger)
    : BaseRepository<Category>(context.Categories), ICategoryRepository
{
    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        if (context.Categories.Count == 0)
        {
            logger.LogInformation("No categories found. Seeding default categories...");

            await AddAsync(new Category(Guid.NewGuid(), "Salary", TransactionType.Income, null), cancellationToken);
            await AddAsync(new Category(Guid.NewGuid(), "Food", TransactionType.Expense, 700m), cancellationToken);

            await context.SaveChangesAsync(cancellationToken);

            logger.LogInformation("Default categories seeded successfully.");
        }
    }
}
