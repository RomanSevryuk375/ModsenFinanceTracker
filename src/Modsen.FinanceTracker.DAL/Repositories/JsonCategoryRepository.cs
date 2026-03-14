using Modsen.FinanceTracker.DAL.Context;
using Modsen.FinanceTracker.Domain.Entities;
using Modsen.FinanceTracker.Domain.Enums;
using Modsen.FinanceTracker.Domain.Interfaces;

namespace Modsen.FinanceTracker.DAL.Repositories;

public class JsonCategoryRepository : BaseRepository<Category>, ICategoryRepository
{
    private readonly JsonDbContext _context;

    public JsonCategoryRepository(JsonDbContext context) : base(context.Categories)
    {
        _context = context;
    }

    public async Task SeedAsync()
    {
        if (!_context.Categories.Any())
        {
            await AddAsync(new Category(Guid.NewGuid(), "Salary", TransactionType.Income));
            await AddAsync(new Category(Guid.NewGuid(), "Food", TransactionType.Expense));
            await _context.SaveChangesAsync();
        }
    }
}