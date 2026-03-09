namespace Modsen.FinanceTracker.Domain.Interfaces;

public interface IRepository<T> where T : class
{
    IEnumerable<T> GetAll();
    void Add(T entity);
    void Delete(Guid id);
}