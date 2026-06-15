using System.Linq.Expressions;

namespace Modsen.FinanceTracker.Domain.Interfaces;

public interface IRepository<T> where T : class, IEntity
{
    public Task<IEnumerable<T>> GetAllAsync(
        Expression<Func<T, bool>>? filter = null, 
        int? skip = null, 
        int? take = null,
        CancellationToken cancellationToken = default);

    public Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    public Task AddAsync(T entity, CancellationToken cancellationToken = default);
    public Task UpdateAsync(T entity, CancellationToken cancellationToken = default);
    public Task DeleteAsync(Guid id,  CancellationToken cancellationToken = default);
}