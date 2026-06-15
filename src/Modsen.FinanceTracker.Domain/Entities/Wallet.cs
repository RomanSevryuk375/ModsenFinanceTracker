using System.Text.Json.Serialization;
using Modsen.FinanceTracker.Domain.Interfaces;

namespace Modsen.FinanceTracker.Domain.Entities;

public sealed class Wallet : IEntity
{
    private const int CurrencyMaxLength = 3;

    [JsonInclude]
    [JsonPropertyName("Transactions")]
    private List<Transaction> Transactions { get; set; } = [];

    public Guid Id { get; init; }

    [JsonInclude]
    public string Name { get; private set; } = string.Empty;

    public decimal Balance => CalculateBalance(Transactions);

    [JsonInclude]
    public string BaseCurrency { get; private set; } = string.Empty;

    [JsonIgnore]
    public IReadOnlyList<Transaction> MutableTransactions => Transactions.AsReadOnly();

    public Wallet() { }

    private Wallet(Guid id, string name, string baseCurrency)
    {
        Id = id;
        Name = name;
        BaseCurrency = baseCurrency;
    }

    public static Result<Wallet> Create(string name, string baseCurrency)
    {
        if (baseCurrency.Trim().Length != CurrencyMaxLength)
        {
            return Result.Fail<Wallet>("Currency must have 3 symbols.");
        }

        return Result.Success(new Wallet(Guid.NewGuid(), name.Trim(), baseCurrency.ToUpper()));
    }

    public Result AddTransaction(Transaction transaction)
    {
        if (IsExpensValid(transaction))
        {
            return Result.Fail("Fail to add transaction. Balance wil become negative.");
        }

        Transactions.Add(transaction);
        return Result.Success();
    }

    public Result RemoveTransaction(Transaction transaction)
    {
        if (ImpossibleToRemove(transaction))
        {
            return Result.Fail("Cannot remove income: wallet balance would become negative.");
        }

        Transactions.Remove(transaction);
        return Result.Success();
    }

    public Result UpdateTransaction(Transaction transaction, decimal newAmount, string newDescription)
    {

        if (transaction is ExpenseTransaction)
        {
            decimal expenseIncrease = newAmount - transaction.Amount;
            if (Balance - expenseIncrease < 0)
            {
                return Result.Fail("Cannot update expense: wallet balance would become negative.");
            }
        }
        else if (transaction is IncomeTransaction)
        {
            decimal incomeDecrease = transaction.Amount - newAmount;
            if (Balance - incomeDecrease < 0)
            {
                return Result.Fail("Cannot reduce income: wallet balance would become negative.");
            }
        }

        Result updateResult = transaction.UpdateDetails(newAmount, newDescription);

        return updateResult;
    }

    private bool ImpossibleToRemove(Transaction transaction) =>
        transaction is IncomeTransaction && (Balance - transaction.Amount < 0);

    private static decimal CalculateBalance(List<Transaction> transactions)
    {
        return transactions.OfType<IncomeTransaction>().Sum(x => x.Amount) -
        transactions.OfType<ExpenseTransaction>().Sum(x => x.Amount);
    }

    private bool IsExpensValid(Transaction transaction) =>
        transaction is ExpenseTransaction && Balance - transaction.Amount < 0;
}
