using Modsen.FinanceTracker.Domain.Extensions;

namespace Modsen.FinanceTracker.BLL.Validators;

public sealed class TransactionValidator : IValidator<Transaction>
{
    public Result<Transaction> Validate(Transaction t)
    {
        if (t.Amount <= 0)
        {
            return Result.Fail<Transaction>("Amount must be positive.");
        }

        if (string.IsNullOrWhiteSpace(t.Description))
        {
            return Result.Fail<Transaction>("Description cannot be empty.");
        }

        if (t.Date > DateTime.Now)
        {
            return Result.Fail<Transaction>("Date cannot be in the future.");
        }

        return Result.Success(t);
    }
}