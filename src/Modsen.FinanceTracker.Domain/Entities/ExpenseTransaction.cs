namespace Modsen.FinanceTracker.Domain.Entities;

public sealed class ExpenseTransaction(
    Guid id, 
    decimal amount, 
    string description, 
    DateTime date, 
    Category category) : Transaction(id, amount, description, date, category) 
{
}