using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions; 
using Modsen.FinanceTracker.DAL.Context;
using Modsen.FinanceTracker.Domain.Entities;

namespace Modsen.FinanceTracker.Tests.DAL.Context;

public sealed class JsonDbContextTests
{
    [Fact]
    public async Task SaveChangesAsyncWhenFileWriteFailsShouldKeepOriginalFileIntactAsync()
    {
        // Arrange
        string fileName = $"test_db_{Guid.NewGuid()}.json";
        string filePath = Path.Combine(Path.GetTempPath(), fileName);
        string tempPath = filePath + ".tmp";
        string backupPath = filePath + ".bak";

        string originalContent = "{\"Wallets\":[],\"Categories\":[]}";

        await File.WriteAllTextAsync(filePath, originalContent);

        ILogger<JsonDbContext> logger = NullLogger<JsonDbContext>.Instance;
        var context = new JsonDbContext(filePath, logger);

        await context.LoadAsync();

        Result<Wallet> walletResult = Wallet.Create("Test Wallet", "USD");
        context.Wallets.Add(walletResult.Value);

        using (FileStream fs = File.Open(tempPath, FileMode.Create, FileAccess.ReadWrite, FileShare.None))
        {
            // Act
            Func<Task> act = async () => await context.SaveChangesAsync(CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<IOException>();
        }

        string currentContent = await File.ReadAllTextAsync(filePath);
        currentContent.Should().Be(originalContent);

        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }

        if (File.Exists(tempPath))
        {
            File.Delete(tempPath);
        }

        if (File.Exists(backupPath))
        {
            File.Delete(backupPath);
        }
    }
}
