namespace FinanceTracker.Tests.DomainLayer;

public sealed class MoneyTests
{
    [Fact]
    public void Create_WithValidCurrency_ShouldReturnSuccessResult()
    {
        // Arrange
        decimal amount = 100.50m;
        string currency = "USD";

        // Act
        Result<Money> result = Money.Create(amount, currency);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Amount.Should().Be(amount);
        result.Value.Currency.Should().Be("USD");
    }

    [Theory]
    [InlineData("US")]
    [InlineData("USDE")]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithInvalidCurrencyLength_ShouldReturnFailureResult(string invalidCurrency)
    {
        // Act
        Result<Money> result = Money.Create(100m, invalidCurrency);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be("Currency must be exactly 3 characters.");
    }

    [Fact]
    public void OperatorAdd_WithSameCurrency_ShouldReturnSum()
    {
        // Arrange
        Money money1 = Money.Create(100m, "USD").Value;
        Money money2 = Money.Create(50m, "USD").Value;

        // Act
        Money sum = money1 + money2;

        // Assert
        sum.Amount.Should().Be(150m);
        sum.Currency.Should().Be("USD");
    }

    [Fact]
    public void OperatorAdd_WithDifferentCurrencies_ShouldThrowInvalidOperationException()
    {
        // Arrange
        Money money1 = Money.Create(100m, "USD").Value;
        Money money2 = Money.Create(50m, "EUR").Value;

        // Act
        Action act = () => { _ = money1 + money2; };

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("Cannot add EUR to USD");
    }

    [Fact]
    public void OperatorSubtract_WithDifferentCurrencies_ShouldThrowInvalidOperationException()
    {
        // Arrange
        Money money1 = Money.Create(100m, "USD").Value;
        Money money2 = Money.Create(50m, "EUR").Value;

        // Act
        Action act = () => { _ = money1 - money2; };

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("Cannot subtract EUR from USD");
    }

    [Fact]
    public void OperatorCompare_LessThan_ShouldCorrectlyCompare()
    {
        // Arrange
        Money money1 = Money.Create(50m, "USD").Value;
        Money money2 = Money.Create(100m, "USD").Value;

        // Act & Assert
        (money1 < money2).Should().BeTrue();
        (money1 > money2).Should().BeFalse();
        (money1 <= money2).Should().BeTrue();
        (money1 >= money2).Should().BeFalse();
    }

    [Fact]
    public void OperatorCompare_WithDifferentCurrencies_ShouldThrowInvalidOperationException()
    {
        // Arrange
        Money money1 = Money.Create(50m, "USD").Value;
        Money money2 = Money.Create(100m, "EUR").Value;

        // Act
        Action actLessThan = () => { _ = money1 < money2; };
        Action actGreaterThan = () => { _ = money1 > money2; };

        // Assert
        actLessThan.Should().Throw<InvalidOperationException>()
            .WithMessage("Cannot compare different currencies");

        actGreaterThan.Should().Throw<InvalidOperationException>()
            .WithMessage("Cannot compare different currencies");
    }
}
