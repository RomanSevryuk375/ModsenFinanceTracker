using Modsen.FinanceTracker.Domain.Extensions;
using Modsen.FinanceTracker.Domain.ValueObjects;

namespace Modsen.FinanceTracker.Domain.Entities;

public class RecurringTransactionTemplate : IEntity
{
    public Guid Id { get; init; }
    [JsonInclude]
    public Money Amount { get; private set; }
    [JsonInclude]
    public string Name { get; private set; }
    [JsonInclude]
    public TransactionDescription Description { get; private set; }
    [JsonInclude]
    public Period Period { get; private set; }
    [JsonInclude]
    public DateTime NextExecutionDate { get; private set; }
    [JsonInclude]
    public Category Category { get; private set; }

    [JsonConstructor]
    private RecurringTransactionTemplate()
    {
        Name = null!;
        Description = null!;
        Category = null!;
        Amount = null!; 
    }
    private RecurringTransactionTemplate(
        Guid id,
        Money amount,
        string name,
        TransactionDescription description,
        Period period,
        DateTime nextExecutionDate,
        Category category)
    {
        Id = id;
        Amount = amount;
        Name = name;
        Description = description;
        Period = period;
        NextExecutionDate = nextExecutionDate;
        Category = category;
    }

    public static Result<RecurringTransactionTemplate> Create(
        Money amount,
        string name,
        TransactionDescription description,
        Period period,
        DateTime nextExecutionDate,
        Category category)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return Result.Fail<RecurringTransactionTemplate>("Name is required");
        }

        var teamplate = new RecurringTransactionTemplate(
            Guid.NewGuid(),
            amount,
            name.Trim(),
            description,
            period,
            nextExecutionDate,
            category);

        return Result.Success<RecurringTransactionTemplate>(teamplate);
    }

    public Result MoveToNextPeriod()
    {
        NextExecutionDate = Period switch
        {
            Period.Daily => NextExecutionDate.AddDays(1),
            Period.Weekly => NextExecutionDate.AddDays(7),
            Period.Monthly => NextExecutionDate.AddMonths(1),

            _ => throw new ArgumentOutOfRangeException(nameof(Period), "Unknown period")
        };

        return Result.Success();
    }
}
