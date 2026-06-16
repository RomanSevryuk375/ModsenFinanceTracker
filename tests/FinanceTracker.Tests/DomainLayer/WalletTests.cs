using Modsen.FinanceTracker.Domain.Entities;
using Modsen.FinanceTracker.Domain.Enums;

namespace FinanceTracker.Tests.DomainLayer;

public sealed class WalletTests
{
    private static Category CreateTestCategory(TransactionType type) =>
        new(Guid.NewGuid(), "TestCategory", type, null);

    private static IncomeTransaction CreateIncome(decimal amount, string currency) =>
        new(Guid.NewGuid(),
            Money.Create(amount, currency).Value,
            TransactionDescription.Create("Test Income").Value,
            TransactionDate.Create(DateTime.Now).Value,
            CreateTestCategory(TransactionType.Income));

    private static ExpenseTransaction CreateExpense(decimal amount, string currency) =>
        new(Guid.NewGuid(),
            Money.Create(amount, currency).Value,
            TransactionDescription.Create("Test Expense").Value,
            TransactionDate.Create(DateTime.Now).Value,
            CreateTestCategory(TransactionType.Expense));

    [Theory]
    [InlineData("US")]
    [InlineData("USDE")]
    [InlineData("")]
    public void Create_WithInvalidCurrency_ShouldReturnFailureResult(string invalidCurrency)
    {
        // Act
        Result<Wallet> result = Wallet.Create("Wallet", invalidCurrency);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be("Currency must have 3 symbols.");
    }

    [Fact]
    public void AddTransaction_IncomeTransaction_ShouldIncreaseBalance()
    {
        // Arrange
        Wallet wallet = Wallet.Create("Main", "USD").Value;
        IncomeTransaction income = CreateIncome(1000m, "USD");

        // Act
        Result result = wallet.AddTransaction(income);

        // Assert
        result.IsSuccess.Should().BeTrue();
        wallet.Balance.Amount.Should().Be(1000m);
    }

    [Fact]
    public void AddTransaction_ExpenseExceedingBalance_ShouldReturnFailureAndNotModifyBalance()
    {
        // Arrange
        Wallet wallet = Wallet.Create("Main", "USD").Value;
        wallet.AddTransaction(CreateIncome(100m, "USD")); 
        ExpenseTransaction expense = CreateExpense(150m, "USD");

        // Act
        Result result = wallet.AddTransaction(expense);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be("Fail to add transaction. Balance wil become negative.");
        wallet.Balance.Amount.Should().Be(100m); 
    }

    [Fact]
    public void RemoveTransaction_IncomeExceedingRemainingBalance_ShouldReturnFailure()
    {
        // Arrange
        Wallet wallet = Wallet.Create("Main", "USD").Value;
        IncomeTransaction income = CreateIncome(150m, "USD");
        ExpenseTransaction expense = CreateExpense(100m, "USD");
        wallet.AddTransaction(income);
        wallet.AddTransaction(expense); 

        // Act
        Result result = wallet.RemoveTransaction(income);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be("Cannot remove income: wallet balance would become negative.");
        wallet.Balance.Amount.Should().Be(50m); 
    }

    [Fact]
    public void RemoveTransaction_ValidTransaction_ShouldSuccessfullyRecalculateBalance()
    {
        // Arrange
        Wallet wallet = Wallet.Create("Main", "USD").Value;
        IncomeTransaction income = CreateIncome(1200m, "USD");
        ExpenseTransaction expense = CreateExpense(200m, "USD");
        wallet.AddTransaction(income);
        wallet.AddTransaction(expense); 

        // Act
        Result result = wallet.RemoveTransaction(expense);

        // Assert
        result.IsSuccess.Should().BeTrue();
        wallet.Balance.Amount.Should().Be(1200m); 
    }

    [Fact]
    public void UpdateTransaction_ExpenseWithInvalidAmount_ShouldReturnFailureAndNotApplyChanges()
    {
        // Arrange
        Wallet wallet = Wallet.Create("Main", "USD").Value;
        IncomeTransaction income = CreateIncome(80m, "USD");
        ExpenseTransaction expense = CreateExpense(30m, "USD");
        wallet.AddTransaction(income);
        wallet.AddTransaction(expense); 

        Money newAmount = Money.Create(90m, "USD").Value;
        TransactionDescription newDescription = TransactionDescription.Create("Updated Expense").Value;

        // Act
        Result result = wallet.UpdateTransaction(expense, newAmount, newDescription);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be("Cannot update expense: wallet balance would become negative.");
        expense.Amount.Amount.Should().Be(30m); 
    }

    [Fact]
    public void UpdateTransaction_ValidAmount_ShouldSuccessfullyUpdateAndModifyBalance()
    {
        // Arrange
        Wallet wallet = Wallet.Create("Main", "USD").Value;
        IncomeTransaction income = CreateIncome(600m, "USD");
        ExpenseTransaction expense = CreateExpense(100m, "USD");
        wallet.AddTransaction(income);
        wallet.AddTransaction(expense); 

        Money newAmount = Money.Create(200m, "USD").Value;
        TransactionDescription newDescription = TransactionDescription.Create("Updated Expense").Value;

        // Act
        Result result = wallet.UpdateTransaction(expense, newAmount, newDescription);

        // Assert
        result.IsSuccess.Should().BeTrue();
        expense.Amount.Amount.Should().Be(200m); 
        wallet.Balance.Amount.Should().Be(400m); 
    }
}
