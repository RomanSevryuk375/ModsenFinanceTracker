using Modsen.FinanceTracker.Domain.Entities;
using Modsen.FinanceTracker.Domain.Interfaces;

namespace Modsen.FinanceTracker.BLL.Services;

public class FinanceService
{
    private readonly IRepository<Transaction> _repository;

    public FinanceService(IRepository<Transaction> repository)
    {
        _repository = repository;
    }

    public void AddTransaction(Transaction transaction) => _repository.Add(transaction);

    public void DeleteTransaction(Guid id) => _repository.Delete(id);

    public decimal GetBalance()
    {
        return _repository.GetAll().Sum(t => t is IncomeTransaction ? t.Amount : -t.Amount);
    }
    
    public IEnumerable<Transaction> GetFilteredTransactions(string? searchTerm, DateTime? from, DateTime? to)
    {
        var query = _repository.GetAll();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            query = query.Where(t => t.Description.Contains(searchTerm, StringComparison.OrdinalIgnoreCase));
        }

        if (from.HasValue)
        {
            query = query.Where(t => t.Date >= from.Value);
        }

        if (to.HasValue)
        {
            query = query.Where(t => t.Date <= to.Value);
        }

        return query.OrderByDescending(t => t.Date);
    }
}