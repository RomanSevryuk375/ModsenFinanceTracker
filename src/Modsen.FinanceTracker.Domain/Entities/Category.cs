namespace Modsen.FinanceTracker.Domain.Entities;

public sealed class Category : IEntity
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public TransactionType Type { get; set; }
    public decimal? BudgetLimit { get; set; }

    public Category(Guid id, string name, TransactionType type, decimal? budgetLimit)
    {
        Id = id;
        Name = name;
        Type = type;
        BudgetLimit = budgetLimit;
    }
}