namespace Modsen.FinanceTracker.Domain.Entities;

public class IncomeTransaction : Transaction 
{
    public IncomeTransaction(Guid id, decimal amount, string description, DateTime date, Guid categoryId) 
        : base(id,
            amount,
            description,
            date,
            categoryId) { }
}