using Modsen.FinanceTracker.BLL.Interfaces;
using Modsen.FinanceTracker.Domain.Entities;

namespace Modsen.FinanceTracker.BLL.Validators;

public class TransactionValidator : IValidator<Transaction>
{
    public (bool IsValid, string Message) Validate(Transaction t)
    {
        if (t.Amount <= 0)
        {
            return (false, "Amount must be positive.");
        }

        if (string.IsNullOrWhiteSpace(t.Description))
        {
            return (false, "Description cannot be empty.");
        }

        if (t.Date > DateTime.Now)
        {
            return (false, "Date cannot be in the future.");
        }

        return (true, string.Empty);
    }
}