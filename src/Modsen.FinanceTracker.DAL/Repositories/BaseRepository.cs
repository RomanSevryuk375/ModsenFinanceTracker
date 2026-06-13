using System.Linq.Expressions;
using Modsen.FinanceTracker.Domain.Interfaces;

namespace Modsen.FinanceTracker.DAL.Repositories;

public abstract class BaseRepository<T>(
    List<T> storageTable) : IRepository<T> where T : class, IEntity
{
    public virtual Task<IEnumerable<T>> GetAllAsync(
        Expression<Func<T, bool>>? filter = null,
        int? skip = null,
        int? take = null,
        CancellationToken cancellationToken = default)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            return Task.FromCanceled<IEnumerable<T>>(cancellationToken);
        }

        var query = storageTable.AsQueryable();

        if (filter is not null)
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

    public virtual Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            return Task.FromCanceled<T?>(cancellationToken);
        }

        var result = storageTable.FirstOrDefault(x => x.Id == id);
        return Task.FromResult(result);
    }

    public virtual Task AddAsync(T entity, CancellationToken cancellationToken = default)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            return Task.FromCanceled(cancellationToken);
        }

        storageTable.Add(entity);
        return Task.CompletedTask;
    }

    public virtual Task UpdateAsync(T entity, CancellationToken cancellationToken = default)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            return Task.FromCanceled(cancellationToken);
        }

        var index = storageTable.FindIndex(x => x.Id == entity.Id);
        if (index is not -1)
        {
            storageTable[index] = entity;
        }
        return Task.CompletedTask;
    }

    public virtual Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            return Task.FromCanceled(cancellationToken);
        }

        var existing = storageTable.FirstOrDefault(x => x.Id == id);
        if (existing is not null)
        {
            storageTable.Remove(existing);
        }
        return Task.CompletedTask;
    }
}