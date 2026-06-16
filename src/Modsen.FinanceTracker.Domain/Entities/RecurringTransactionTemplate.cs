using Modsen.FinanceTracker.Domain.Extensions;

namespace Modsen.FinanceTracker.Domain.Entities;

public class RecurringTransactionTemplate : IEntity
{
    public Guid Id { get; init; }
    [JsonInclude]
    public decimal Amount { get; private set; }
    [JsonInclude]
    public string Name { get; private set; }
    [JsonInclude]
    public string Description { get; private set; }
    [JsonInclude]
    public Period Period { get; private set; }
    [JsonInclude]
    public DateTime NextExecutionDate { get; private set; }
    [JsonInclude]
    public Category Category { get; private set; }

    [JsonConstructor]
    private RecurringTransactionTemplate() { Description = string.Empty; Name = string.Empty; Category = null!; }
    private RecurringTransactionTemplate(
        Guid id,
        decimal amount,
        string name,
        string description,
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
        decimal amount,
        string name,
        string description,
        Period period,
        DateTime nextExecutionDate,
        Category category)
    {
        if(amount <= 0)
        {
            return Result.Fail<RecurringTransactionTemplate>("Amount must be strictly positive");
        }

        if (string.IsNullOrWhiteSpace(description))
        {
            return Result.Fail<RecurringTransactionTemplate>("Description is required");
        }

        if (string.IsNullOrWhiteSpace(name))
        {
            return Result.Fail<RecurringTransactionTemplate>("Name is required");
        }

        var teamplate = new RecurringTransactionTemplate(
            Guid.NewGuid(),
            amount,
            name.Trim(),
            description.Trim(),
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
