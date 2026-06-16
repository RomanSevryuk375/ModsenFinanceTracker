using Modsen.FinanceTracker.Domain.Extensions;

namespace Modsen.FinanceTracker.Domain.ValueObjects;

public sealed record TransactionDate
{
    [JsonInclude]
    public DateTime Value { get; private set; }

    [JsonConstructor]
    private TransactionDate(DateTime value)
    {
        Value = value;
    }

    public static Result<TransactionDate> Create(DateTime date)
    {
        if (date > DateTime.Now)
        {
            return Result.Fail<TransactionDate>("Transaction date cannot be in the future.");
        }

        return Result.Success(new TransactionDate(date));
    }

    public override string ToString() => Value.ToShortDateString();
}
