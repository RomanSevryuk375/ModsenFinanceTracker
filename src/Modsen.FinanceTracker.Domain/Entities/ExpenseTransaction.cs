namespace Modsen.FinanceTracker.Domain.Entities;

public class ExpenseTransaction : Transaction 
{
    public ExpenseTransaction(Guid id, decimal amount, string description, DateTime date, Guid categoryId) 
        : base(id, amount, description, date, categoryId) { }
}