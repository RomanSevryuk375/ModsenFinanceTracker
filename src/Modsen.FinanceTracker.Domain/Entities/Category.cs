namespace Modsen.FinanceTracker.Domain.Entities;

public sealed class Category : IEntity
{
    public Guid Id { get; init; }
    public string Name { get; init; }
    public TransactionType Type { get; init; }
    public decimal? BudgetLimit { get; init; }

    public Category(Guid id, string name, TransactionType type, decimal? budgetLimit)
    {
        Id = id;
        Name = name;
        Type = type;
        BudgetLimit = budgetLimit;
    }
}
