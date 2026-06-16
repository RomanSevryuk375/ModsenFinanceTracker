using Modsen.FinanceTracker.Domain.Extensions;

namespace Modsen.FinanceTracker.Domain.ValueObjects;

public sealed record Money : IComparable<Money>
{
    [JsonInclude]
    public decimal Amount { get; private set; }
    [JsonInclude]
    public string Currency { get; private set; }

    [JsonConstructor]
    private Money(decimal amount, string currency)
    {
        Amount = amount;
        Currency = currency;
    }

    public static Result<Money> Create(decimal amount, string currency)
    {
        if (string.IsNullOrWhiteSpace(currency) || currency.Trim().Length != 3)
        {
            return Result.Fail<Money>("Currency must be exactly 3 characters.");
        }

        return Result.Success(new Money(amount, currency.Trim().ToUpper()));
    }

    public static Money operator +(Money a, Money b)
    {
        if (a.Currency != b.Currency)
        {
            throw new InvalidOperationException($"Cannot add {b.Currency} to {a.Currency}");
        }

        return new Money(a.Amount + b.Amount, a.Currency);
    }

    public static Money operator -(Money a, Money b)
    {
        if (a.Currency != b.Currency)
        {
            throw new InvalidOperationException($"Cannot subtract {b.Currency} from {a.Currency}");
        }

        return new Money(a.Amount - b.Amount, a.Currency);
    }

    public int CompareTo(Money? other)
    {
        if (other is null)
        {
            return 1;
        }

        if (Currency != other.Currency)
        {
            throw new InvalidOperationException("Cannot compare different currencies");
        }

        return Amount.CompareTo(other.Amount);
    }

    public static bool operator >(Money a, Money b) => a.CompareTo(b) > 0;
    public static bool operator <(Money a, Money b) => a.CompareTo(b) < 0;
    public static bool operator >=(Money a, Money b) => a.CompareTo(b) >= 0;
    public static bool operator <=(Money a, Money b) => a.CompareTo(b) <= 0;

    public override string ToString() => $"{Amount:N2} {Currency}";
}
