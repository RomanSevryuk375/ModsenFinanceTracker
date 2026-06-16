using Modsen.FinanceTracker.Domain.Extensions;

namespace Modsen.FinanceTracker.Domain.ValueObjects;

public sealed record TransactionDescription
{
    private const int MaxLength = 500;

    [JsonInclude]
    public string Value { get; private set; }

    [JsonConstructor]
    private TransactionDescription(string value)
    {
        Value = value;
    }

    public static Result<TransactionDescription> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return Result.Fail<TransactionDescription>("Description cannot be empty.");
        }

        if (value.Length > MaxLength)
        {
            return Result.Fail<TransactionDescription>($"Description cannot exceed {MaxLength} characters.");
        }

        return Result.Success(new TransactionDescription(value.Trim()));
    }

    public override string ToString() => Value;
}
