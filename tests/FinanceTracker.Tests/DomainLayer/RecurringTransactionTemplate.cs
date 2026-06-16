using Modsen.FinanceTracker.Domain.Entities;
using Modsen.FinanceTracker.Domain.Enums;

namespace FinanceTracker.Tests.DomainLayer;

public sealed class RecurringTransactionTemplateTests
{
    private static Category CreateTestCategory() =>
        new(Guid.NewGuid(), "Netflix", TransactionType.Expense, null);

    [Fact]
    public void MoveToNextPeriod_Daily_ShouldAddOneDay()
    {
        // Arrange
        DateTime nextDate = new(2026, 6, 15);
        Category category = CreateTestCategory();
        RecurringTransactionTemplate template = RecurringTransactionTemplate.Create(
            Money.Create(10m, "USD").Value,
            "Netflix Subscription",
            TransactionDescription.Create("Daily Sub").Value,
            Period.Daily,
            nextDate,
            category).Value;

        // Act
        Result result = template.MoveToNextPeriod();

        // Assert
        result.IsSuccess.Should().BeTrue();
        template.NextExecutionDate.Should().Be(new DateTime(2026, 6, 16));
    }

    [Fact]
    public void MoveToNextPeriod_Monthly_ShouldAddOneMonth()
    {
        // Arrange
        DateTime nextDate = new(2026, 6, 15);
        Category category = CreateTestCategory();
        RecurringTransactionTemplate template = RecurringTransactionTemplate.Create(
            Money.Create(10m, "USD").Value,
            "Netflix Subscription",
            TransactionDescription.Create("Monthly Sub").Value,
            Period.Monthly,
            nextDate,
            category).Value;

        // Act
        Result result = template.MoveToNextPeriod();

        // Assert
        result.IsSuccess.Should().BeTrue();
        template.NextExecutionDate.Should().Be(new DateTime(2026, 7, 15));
    }
}
