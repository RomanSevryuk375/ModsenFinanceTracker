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

    public virtual async Task<IEnumerable<T>> GetAllAsync(
        Expression<Func<T, bool>>? filter = null,
        int? skip = null,
        int? take = null)
    {
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

        return await Task.FromResult(query.ToList());
    }

    public virtual async Task<T?> GetByIdAsync(Guid id)
    {
        return await Task.FromResult(_storageTable.FirstOrDefault(x => x.Id == id));
    }

    public virtual async Task AddAsync(T entity)
    {
        await Task.Run(() => _storageTable.Add(entity));
    }

    public virtual async Task UpdateAsync(T entity)
    {
        await Task.Run(() =>
        {
            var updateEntity = entity;
            var id = updateEntity.Id;
            
            var existing = _storageTable.FirstOrDefault(x => x.Id == id);

            if (existing is not null)
            {
                var index = _storageTable.IndexOf(existing);
                _storageTable[index] = entity;
            }
        });
    }

    public virtual async Task DeleteAsync(Guid id)
    {
        await Task.Run(() =>
        {
            var existing = _storageTable.FirstOrDefault(x => x.Id == id);

            if (existing != null)
            {
                _storageTable.Remove(existing);
            }
        });
    }
}