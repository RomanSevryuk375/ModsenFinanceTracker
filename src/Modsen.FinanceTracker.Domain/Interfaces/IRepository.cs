using System.Linq.Expressions;

namespace Modsen.FinanceTracker.Domain.Interfaces;

public interface IRepository<T> where T : class, IEntity
{
    Task<IEnumerable<T>> GetAllAsync(
        Expression<Func<T, bool>>? filter = null, 
        int? skip = null, 
        int? take = null,
        CancellationToken cancellationToken = default);
    
    Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddAsync(T entity, CancellationToken cancellationToken = default);
    Task UpdateAsync(T entity, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id,  CancellationToken cancellationToken = default);
}