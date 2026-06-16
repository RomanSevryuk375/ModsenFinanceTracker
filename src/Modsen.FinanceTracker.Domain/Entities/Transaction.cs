using Modsen.FinanceTracker.Domain.Extensions;
using Modsen.FinanceTracker.Domain.ValueObjects;

namespace Modsen.FinanceTracker.Domain.Entities;

[JsonDerivedType(typeof(IncomeTransaction), typeDiscriminator: "income")]
[JsonDerivedType(typeof(ExpenseTransaction), typeDiscriminator: "expense")]
public abstract class Transaction : IEntity
{
    public Guid Id { get; init; }

    [JsonInclude]
    public Money Amount { get; private set; }

    [JsonInclude]
    public TransactionDescription Description { get; private set; }

    [JsonInclude]
    public TransactionDate Date { get; private set; }

    [JsonInclude]
    public Category Category { get; private set; }

    protected Transaction(
        Guid id,
        Money amount,
        TransactionDescription description,
        TransactionDate date,
        Category category)
    {
        Id = id;
        Amount = amount;
        Description = description;
        Date = date;
        Category = category;
    }

    public Result UpdateDetails(Money newAmount, TransactionDescription newDescription)
    {
        Amount = newAmount;
        Description = newDescription;

        return Result.Success();
    }
}
