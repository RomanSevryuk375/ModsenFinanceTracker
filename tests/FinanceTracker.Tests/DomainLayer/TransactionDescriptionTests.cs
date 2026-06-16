namespace FinanceTracker.Tests.DomainLayer;

public sealed class TransactionDescriptionTests
{
    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Create_WithEmptyValue_ShouldReturnFailureResult(string? emptyValue)
    {
        // Act
        Result<TransactionDescription> result = TransactionDescription.Create(emptyValue!);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be("Description cannot be empty.");
    }

    [Fact]
    public void Create_WithValueExceedingMaxLength_ShouldReturnFailureResult()
    {
        // Arrange
        string longValue = new('a', 501);

        // Act
        Result<TransactionDescription> result = TransactionDescription.Create(longValue);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be("Description cannot exceed 500 characters.");
    }

    [Fact]
    public void Create_WithValidValue_ShouldReturnTrimmedSuccessResult()
    {
        // Arrange
        string validValue = "  Lunch at cafe  ";

        // Act
        Result<TransactionDescription> result = TransactionDescription.Create(validValue);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Value.Should().Be("Lunch at cafe");
    }
}
