using Microsoft.Extensions.Logging;
using Modsen.FinanceTracker.BLL.Interfaces;
using Modsen.FinanceTracker.BLL.Services;
using Modsen.FinanceTracker.Domain.Entities;
using Modsen.FinanceTracker.Domain.Enums;
using Modsen.FinanceTracker.Domain.Interfaces;

namespace FinanceTracker.Tests.BusinessLogicLayer;

public sealed class SchedulerServiceTests
{
    private readonly IWalletRepository _walletRepository = Substitute.For<IWalletRepository>();
    private readonly ITransactionFactory _transactionFactory = Substitute.For<ITransactionFactory>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly ILogger<SchedulerService> _logger = Substitute.For<ILogger<SchedulerService>>();

    private readonly SchedulerService _sut;

    public SchedulerServiceTests()
    {
        _sut = new SchedulerService(_walletRepository, _transactionFactory, _unitOfWork, _logger);
    }

    private static Category CreateTestCategory() =>
        new(Guid.NewGuid(), "Netflix", TransactionType.Expense, null);

    private static RecurringTransactionTemplate CreateTemplate(DateTime nextDate, Category category) =>
        RecurringTransactionTemplate.Create(
            Money.Create(15m, "USD").Value,
            "Netflix Sub",
            TransactionDescription.Create("Monthly Sub").Value,
            Period.Monthly,
            nextDate,
            category).Value;

    [Fact]
    public async Task CheckAsyncNoDueTemplatesShouldNotModifyAnythingAsync()
    {
        // Arrange
        Wallet wallet = Wallet.Create("Main", "USD").Value;

        RecurringTransactionTemplate template = CreateTemplate(DateTime.Now.AddMonths(1), CreateTestCategory());
        wallet.AddTransactionTemplate(template);

        _walletRepository.GetAllAsync(null, null, null, Arg.Any<CancellationToken>())
            .Returns(new List<Wallet> { wallet });

        // Act
        Result result = await _sut.CheckAndProcessRecurringTransactionsAsync(CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        wallet.Transactions.Should().BeEmpty(); 
        await _unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>()); 
    }

    [Fact]
    public async Task CheckAsyncTemplateDueShouldCreateTransactionAdvancePeriodAndSaveAsync()
    {
        // Arrange
        Wallet wallet = Wallet.Create("Main", "USD").Value;

        var income = new IncomeTransaction(Guid.NewGuid(), Money.Create(100m, "USD").Value,
            TransactionDescription.Create("Deposit").Value, TransactionDate.Create(DateTime.Now).Value, null!);
        wallet.AddTransaction(income);

        Category category = CreateTestCategory();
        DateTime yesterday = DateTime.Now.AddDays(-1);
        RecurringTransactionTemplate template = CreateTemplate(yesterday, category);
        wallet.AddTransactionTemplate(template);

        _walletRepository.GetAllAsync(null, null, null, Arg.Any<CancellationToken>())
            .Returns(new List<Wallet> { wallet });

        var expectedTransaction = new ExpenseTransaction(
            Guid.NewGuid(),
            template.Amount,
            template.Description,
            TransactionDate.Create(DateTime.Now).Value, category);

        _transactionFactory.CreateTransaction(Arg.Any<TransactionType>(), Arg.Any<Money>(),
            Arg.Any<TransactionDescription>(), Arg.Any<Category>())
            .Returns(expectedTransaction);

        bool eventFired = false;
        _sut.OnTransactionWrittenOff += (sender, args) => eventFired = true;

        // Act
        Result result = await _sut.CheckAndProcessRecurringTransactionsAsync(CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        wallet.Transactions.Should().Contain(expectedTransaction);
        template.NextExecutionDate.Should().BeCloseTo(yesterday.AddMonths(1), TimeSpan.FromMinutes(5));

        eventFired.Should().BeTrue();
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CheckAsyncTemplateDueShouldCreateTransactionAdvancePeriodAndSave()
    {
        // Arrange
        Wallet wallet = Wallet.Create("Main", "USD").Value;

        var income = new IncomeTransaction(Guid.NewGuid(), Money.Create(100m, "USD").Value,
            TransactionDescription.Create("Deposit").Value, TransactionDate.Create(DateTime.Now).Value, null!);
        wallet.AddTransaction(income);

        DateTime yesterday = DateTime.Now.AddDays(-1);

        Category category = CreateTestCategory();
        RecurringTransactionTemplate template = CreateTemplate(yesterday, category);
        wallet.AddTransactionTemplate(template);

        _walletRepository.GetAllAsync(null, null, null, Arg.Any<CancellationToken>())
            .Returns(new List<Wallet> { wallet });

        var expectedTransaction = new ExpenseTransaction(
            Guid.NewGuid(), template.Amount, template.Description, TransactionDate.Create(DateTime.Now).Value, category);

        _transactionFactory.CreateTransaction(Arg.Any<TransactionType>(), Arg.Any<Money>(),
            Arg.Any<TransactionDescription>(), Arg.Any<Category>())
            .Returns(expectedTransaction);

        bool eventFired = false;
        _sut.OnTransactionWrittenOff += (sender, args) => eventFired = true;

        // Act
        Result result = await _sut.CheckAndProcessRecurringTransactionsAsync(CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        wallet.Transactions.Should().Contain(expectedTransaction);
        template.NextExecutionDate.Should().BeCloseTo(yesterday.AddMonths(1), TimeSpan.FromMinutes(5));

        eventFired.Should().BeTrue();
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
