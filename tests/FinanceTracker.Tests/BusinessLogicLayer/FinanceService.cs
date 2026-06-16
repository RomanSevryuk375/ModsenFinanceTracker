using Modsen.FinanceTracker.BLL.Interfaces;
using Modsen.FinanceTracker.BLL.Services;
using Modsen.FinanceTracker.Domain.Entities;
using Modsen.FinanceTracker.Domain.Enums;
using Modsen.FinanceTracker.Domain.Interfaces;

namespace FinanceTracker.Tests.BusinessLogicLayer;

public sealed class FinanceServiceTests
{
    private readonly IWalletRepository _walletRepository = Substitute.For<IWalletRepository>();
    private readonly ICurrencyService _currencyService = Substitute.For<ICurrencyService>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly string _systemCurrency = "USD";

    private readonly FinanceService _sut; 

    public FinanceServiceTests()
    {
        _sut = new FinanceService(
            _walletRepository,
            _currencyService,
            _unitOfWork,
            _systemCurrency);
    }

    private static Category CreateCategory(TransactionType type, decimal? budgetLimit = null) =>
        new(Guid.NewGuid(), "TestCategory", type, budgetLimit);

    private static IncomeTransaction CreateIncome(decimal amount, string currency, Category category) =>
        new(Guid.NewGuid(),
            Money.Create(amount, currency).Value,
            TransactionDescription.Create("Deposit").Value,
            TransactionDate.Create(DateTime.Now).Value,
            category);

    private static ExpenseTransaction CreateExpense(decimal amount, string currency, Category category) =>
        new(Guid.NewGuid(),
            Money.Create(amount, currency).Value,
            TransactionDescription.Create("Expense").Value,
            TransactionDate.Create(DateTime.Now).Value,
            category);

    [Fact]
    public async Task AddTransactionAsyncWalletNotFoundShouldReturnFailureAsync()
    {
        // Arrange
        var walletId = Guid.NewGuid();
        IncomeTransaction transaction = CreateIncome(100m, "USD", CreateCategory(TransactionType.Income));

        _walletRepository.GetByIdAsync(walletId, Arg.Any<CancellationToken>())
            .Returns((Wallet?)null);

        // Act
        Result result = await _sut.AddTransactionAsync(walletId, transaction, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be($"Wallet {walletId} not found.");
    }

    [Fact]
    public async Task AddTransactionAsyncExpenseExceedingBalanceShouldReturnFailureAndNotSaveAsync()
    {
        // Arrange
        Wallet wallet = Wallet.Create("Main", "USD").Value;

        wallet.AddTransaction(CreateIncome(100m, "USD", CreateCategory(TransactionType.Income)));

        _walletRepository.GetByIdAsync(wallet.Id, Arg.Any<CancellationToken>())
            .Returns(wallet);

        ExpenseTransaction invalidExpense = CreateExpense(150m, "USD", CreateCategory(TransactionType.Expense));

        // Act
        Result result = await _sut.AddTransactionAsync(wallet.Id, invalidExpense, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be("Fail to add transaction. Balance wil become negative."); 

        await _unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task DeleteTransactionAsyncValidExecutionShouldCallUnitOfWorkAsync()
    {
        // Arrange
        Wallet wallet = Wallet.Create("Main", "USD").Value;
        IncomeTransaction transaction = CreateIncome(100m, "USD", CreateCategory(TransactionType.Income));
        wallet.AddTransaction(transaction); 

        _walletRepository.GetByIdAsync(wallet.Id, Arg.Any<CancellationToken>())
            .Returns(wallet);

        // Act
        Result result = await _sut.DeleteTransactionAsync(wallet.Id, transaction.Id, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        wallet.Transactions.Should().BeEmpty(); 
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetBalanceAsyncValidExecutionWithCurrencyConversionShouldReturnConvertedBalanceAsync()
    {
        // Arrange
        Wallet wallet = Wallet.Create("Savings", "EUR").Value; 

        wallet.AddTransaction(CreateIncome(100m, "EUR", CreateCategory(TransactionType.Income)));

        _walletRepository.GetByIdAsync(wallet.Id, Arg.Any<CancellationToken>())
            .Returns(wallet);

        _currencyService.GetExchangeRateAsync("EUR", _systemCurrency, Arg.Any<CancellationToken>())
            .Returns(Result.Success(1.10m));

        // Act
        Result<decimal> result = await _sut.GetBalanceAsync(wallet.Id, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(110.00m); 
    }
}
