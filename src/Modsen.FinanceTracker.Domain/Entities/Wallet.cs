using Modsen.FinanceTracker.Domain.Extensions;
using Modsen.FinanceTracker.Domain.ValueObjects;

namespace Modsen.FinanceTracker.Domain.Entities;

public sealed class Wallet : IEntity
{
    private const int CurrencyMaxLength = 3;

    [JsonInclude]
    [JsonPropertyName("Transactions")]
    private List<Transaction> InternalTransactions { get; set; } = [];

    [JsonInclude]
    [JsonPropertyName("TransactionTemplates")]
    private List<RecurringTransactionTemplate> InternalTransactionTemplates { get; set; } = [];

    public Guid Id { get; init; }

    [JsonInclude]
    public string Name { get; private set; } = string.Empty;

    public Money Balance => CalculateBalance(InternalTransactions, BaseCurrency);

    [JsonInclude]
    public string BaseCurrency { get; private set; } = string.Empty;

    [JsonIgnore]
    public IReadOnlyList<Transaction> Transactions => InternalTransactions.AsReadOnly();
    [JsonIgnore]
    public IReadOnlyList<RecurringTransactionTemplate> Templates => InternalTransactionTemplates.AsReadOnly();

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
        if (WouldCauseNegativeBalance(transaction))
        {
            return Result.Fail("Fail to add transaction. Balance wil become negative.");
        }

        InternalTransactions.Add(transaction);
        return Result.Success();
    }

    public Result RemoveTransaction(Transaction transaction)
    {
        if (ImpossibleToRemove(transaction))
        {
            return Result.Fail("Cannot remove income: wallet balance would become negative.");
        }

        InternalTransactions.Remove(transaction);
        return Result.Success();
    }

    public Result UpdateTransaction(
        Transaction transaction,
        Money newAmount,
        TransactionDescription newDescription)
    {

        if (transaction is ExpenseTransaction)
        {
            Money expenseIncrease = newAmount - transaction.Amount;
            if (Balance.Amount - expenseIncrease.Amount < 0)
            {
                return Result.Fail("Cannot update expense: wallet balance would become negative.");
            }
        }
        else if (transaction is IncomeTransaction)
        {
            Money incomeDecrease = transaction.Amount - newAmount;
            if (Balance.Amount - incomeDecrease.Amount < 0)
            {
                return Result.Fail("Cannot reduce income: wallet balance would become negative.");
            }
        }

        Result updateResult = transaction.UpdateDetails(newAmount, newDescription);

        return updateResult;
    }

    public Result AddTransactionTemplate(RecurringTransactionTemplate template)
    {
        InternalTransactionTemplates.Add(template);

        return Result.Success();
    }

    public Result RemoveTransactionTemplate(RecurringTransactionTemplate template)
    {
        InternalTransactionTemplates.Remove(template);

        return Result.Success();
    }

    private bool ImpossibleToRemove(Transaction transaction) =>
        transaction is IncomeTransaction && Balance < transaction.Amount;

    private static Money CalculateBalance(List<Transaction> transactions, string currency)
    {
        decimal total = transactions.OfType<IncomeTransaction>().Sum(x => x.Amount.Amount) -
                        transactions.OfType<ExpenseTransaction>().Sum(x => x.Amount.Amount);

        return Money.Create(total, currency).Value;
    }

    private bool WouldCauseNegativeBalance(Transaction transaction) =>
        transaction is ExpenseTransaction && Balance < transaction.Amount;
}
