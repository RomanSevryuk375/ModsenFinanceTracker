using System.Text.Json.Serialization;
using Modsen.FinanceTracker.Domain.Interfaces;

namespace Modsen.FinanceTracker.Domain.Entities;

[JsonDerivedType(typeof(IncomeTransaction), typeDiscriminator: "income")]
[JsonDerivedType(typeof(ExpenseTransaction), typeDiscriminator: "expense")]
public abstract class Transaction : IEntity
{
    public Guid Id { get; init; }

    [JsonInclude]
    public decimal Amount { get; private set; }

    [JsonInclude]
    public string Description { get; private set; }

    [JsonInclude]
    public DateTime Date { get; private set; }

    [JsonInclude]
    public Category Category { get; private set; }

    protected Transaction(Guid id, decimal amount, string description, DateTime date, Category category)
    {
        if (amount <= 0)
        {
            throw new ArgumentException("Amount must be strictly positive");
        }

        if (string.IsNullOrWhiteSpace(description))
        {
            throw new ArgumentException("Description is required");
        }

        Id = id;
        Amount = amount;
        Description = description;
        Date = date;
        Category = category;
    }

    public Result UpdateDetails(decimal newAmount, string newDescription)
    {
        if (newAmount <= 0)
        {
            return Result.Fail("Amount must be positive");
        }

        Amount = newAmount;
        Description = newDescription;
        return Result.Success();
    }
}