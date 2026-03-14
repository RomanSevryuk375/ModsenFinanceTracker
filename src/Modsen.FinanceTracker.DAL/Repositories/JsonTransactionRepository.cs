using Modsen.FinanceTracker.DAL.Context;
using Modsen.FinanceTracker.Domain.Entities;
using Modsen.FinanceTracker.Domain.Interfaces;

namespace Modsen.FinanceTracker.DAL.Repositories;

public class JsonTransactionRepository : BaseRepository<Transaction>, ITransactionRepository
{
    private readonly JsonDbContext _context;

    public JsonTransactionRepository(JsonDbContext context) : base(context.Transactions)
    {
        _context = context;
    }
    
    public override async Task AddAsync(Transaction entity)
    {
        await base.AddAsync(entity);
        await _context.SaveChangesAsync(); 
    }

    public override async Task UpdateAsync(Transaction entity)
    {
        await base.UpdateAsync(entity);
        await _context.SaveChangesAsync();
    }
    
    public override async Task DeleteAsync(Guid id)
    {
        await base.DeleteAsync(id);
        await _context.SaveChangesAsync();
    }
}