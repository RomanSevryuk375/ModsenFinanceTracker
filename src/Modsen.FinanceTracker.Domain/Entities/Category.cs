using Modsen.FinanceTracker.Domain.Enums;

namespace Modsen.FinanceTracker.Domain.Entities;

public class Category
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public TransactionType Type { get; set; }

    public Category(Guid id, string name, TransactionType type)
    {
        Id = id;
        Name = name;
        Type = type;
    }
}