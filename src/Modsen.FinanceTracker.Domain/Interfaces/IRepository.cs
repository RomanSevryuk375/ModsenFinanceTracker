using System.Linq.Expressions;

namespace Modsen.FinanceTracker.Domain.Interfaces;

public interface IRepository<T> where T : class, IEntity
{
    Task<IEnumerable<T>> GetAllAsync(
        Expression<Func<T, bool>>? filter = null, 
        int? skip = null, 
        int? take = null,
        CancellationToken ct = default);
    
    Task<T?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task AddAsync(T entity, CancellationToken ct = default);
    Task UpdateAsync(T entity, CancellationToken ct = default);
    Task DeleteAsync(Guid id,  CancellationToken ct = default);
}