namespace FinanceTracker.Tests.DomainLayer;

public sealed class TransactionDateTests
{
    [Fact]
    public void Create_WithFutureDate_ShouldReturnFailureResult()
    {
        // Arrange
        DateTime futureDate = DateTime.Now.AddDays(1);

        // Act
        Result<TransactionDate> result = TransactionDate.Create(futureDate);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be("Transaction date cannot be in the future.");
    }

    [Fact]
    public void Create_WithPastDate_ShouldReturnSuccessResult()
    {
        // Arrange
        DateTime pastDate = DateTime.Now.AddDays(-1);

        // Act
        Result<TransactionDate> result = TransactionDate.Create(pastDate);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Value.Should().Be(pastDate);
    }
}
