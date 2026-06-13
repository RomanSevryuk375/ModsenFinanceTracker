namespace Modsen.FinanceTracker.Domain.Entities;

public sealed class ExpenseTransaction(
    Guid id, 
    decimal amount, 
    string description, 
    DateTime date, 
    Guid categoryId) : Transaction(id, amount, description, date, categoryId) 
{
}