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
    
    public override async Task AddAsync(Transaction entity, CancellationToken ct = default)
    {
        await base.AddAsync(entity, ct);
        await _context.SaveChangesAsync(ct); 
    }

    public override async Task UpdateAsync(Transaction entity, CancellationToken ct = default)
    {
        await base.UpdateAsync(entity, ct);
        await _context.SaveChangesAsync(ct);
    }
    
    public override async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        await base.DeleteAsync(id, ct);
        await _context.SaveChangesAsync(ct);
    }
}