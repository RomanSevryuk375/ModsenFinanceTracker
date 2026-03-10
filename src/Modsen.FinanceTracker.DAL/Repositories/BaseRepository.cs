using System.Linq.Expressions;
using Modsen.FinanceTracker.Domain.Interfaces;

namespace Modsen.FinanceTracker.DAL.Repositories;

public abstract class BaseRepository<T> : IRepository<T> where T : class
{
    protected readonly List<T> _storage;

    protected BaseRepository(List<T> storage)
    {
        _storage = storage;
    }

    public virtual async Task<IEnumerable<T>> GetAllAsync(
        Expression<Func<T, bool>>? filter = null,
        int? skip = null,
        int? take = null)
    {
        var query = _storage.AsQueryable();

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

        return await Task.FromResult(query.ToList());
    }

    public virtual async Task<T?> GetByIdAsync(Guid id)
    {
        return await Task.FromResult(_storage.Cast<dynamic>().FirstOrDefault(x => x.Id == id));
    }

    public virtual async Task AddAsync(T entity)
    {
        await Task.Run(() => _storage.Add(entity));
    }

    public virtual async Task UpdateAsync(T entity)
    {
        await Task.Run(() =>
        {
            var dynamicEntity = (dynamic)entity;
            Guid id = dynamicEntity.Id;
            
            var existing = _storage.Cast<dynamic>().FirstOrDefault(x => x.Id == id);

            if (existing is not null)
            {
                var index = _storage.IndexOf((T)existing);
                _storage[index] = entity;
            }
        });
    }

    public virtual async Task DeleteAsync(Guid id)
    {
        await Task.Run(() =>
        {
            var existing = _storage.Cast<dynamic>().FirstOrDefault(x => x.Id == id);

            if (existing != null)
            {
                _storage.Remove((T)existing);
            }
        });
    }
}