using System.Linq.Expressions;
using Modsen.FinanceTracker.Domain.Interfaces;

namespace Modsen.FinanceTracker.DAL.Repositories;

public abstract class BaseRepository<T> : IRepository<T> where T : class, IEntity
{
    protected readonly List<T> _storageTable;

    protected BaseRepository(List<T> storageTable)
    {
        _storageTable = storageTable;
    }
    
    public virtual Task<IEnumerable<T>> GetAllAsync(
        Expression<Func<T, bool>>? filter = null,
        int? skip = null,
        int? take = null,
        CancellationToken ct = default)
    {
        if (ct.IsCancellationRequested)
        {
            return Task.FromCanceled<IEnumerable<T>>(ct);
        }

        var query = _storageTable.AsQueryable();

        if (filter != null)
        {
            query = query.Where(filter);
        }

        if (skip.HasValue)
        {
            query = query.Skip(skip.Value);
        }

        if (take.HasValue)
        {
            query = query.Take(take.Value);
        }

        var result = query.ToList();

        return Task.FromResult<IEnumerable<T>>(result);
    }

    public virtual Task<T?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        if (ct.IsCancellationRequested)
        {
            return Task.FromCanceled<T?>(ct);
        }

        var result = _storageTable.FirstOrDefault(x => x.Id == id);
        return Task.FromResult(result);
    }

    public virtual Task AddAsync(T entity, CancellationToken ct = default)
    {
        if (ct.IsCancellationRequested)
        {
            return Task.FromCanceled(ct);
        }

        _storageTable.Add(entity);
        return Task.CompletedTask;
    }

    public virtual Task UpdateAsync(T entity, CancellationToken ct = default)
    {
        if (ct.IsCancellationRequested)
        {
            return Task.FromCanceled(ct);
        }

        var index = _storageTable.FindIndex(x => x.Id == entity.Id);
        if (index is not -1)
        {
            _storageTable[index] = entity;
        }
        return Task.CompletedTask;
    }

    public virtual Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        if (ct.IsCancellationRequested)
        {
            return Task.FromCanceled(ct);
        }

        var existing = _storageTable.FirstOrDefault(x => x.Id == id);
        if (existing is not null)
        {
            _storageTable.Remove(existing);
        }
        return Task.CompletedTask;
    }
}