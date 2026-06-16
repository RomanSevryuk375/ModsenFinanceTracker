namespace Modsen.FinanceTracker.DAL.Context;

public sealed class JsonDbContext : IDataContext
{
    private readonly string _filePath;
    private readonly JsonSerializerOptions _options;
    private readonly ILogger<JsonDbContext> _logger;
    private readonly SemaphoreSlim _semaphore = new(1, 1);

    public List<Wallet> Wallets { get; private set; } = [];
    public List<Category> Categories { get; private set; } = [];

    public JsonDbContext(string filePath, ILogger<JsonDbContext> logger)
    {
        _filePath = filePath;
        _logger = logger;
        _options = new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNameCaseInsensitive = true,
            IncludeFields = true
        };
    }

    public async Task LoadAsync(CancellationToken cancellationToken = default)
    {
        if (!File.Exists(_filePath))
        {
            _logger.LogInformation("Database file '{FilePath}' not found. Starting with empty database.", _filePath);
            return;
        }

        try
        {
            _logger.LogInformation("Loading database from '{FilePath}'...", _filePath);

            string json = await File.ReadAllTextAsync(_filePath, cancellationToken);
            JsonDataModel? data = JsonSerializer.Deserialize<JsonDataModel>(json, _options);

            if (data is not null)
            {
                Wallets = data.Wallets;
                Categories = data.Categories;
                _logger.LogInformation(
                    "Database loaded successfully. Wallets: {WalletsCount}, Categories: {CategoriesCount}.",
                    Wallets.Count,
                    Categories.Count);
            }
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, "FATAL ERROR: Failed to load database from '{FilePath}'!", _filePath);
            throw;
        }
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await _semaphore.WaitAsync(cancellationToken);
        try
        {
            _logger.LogDebug("Initiating database save to '{FilePath}'...", _filePath);

            var data = new JsonDataModel
            {
                Wallets = Wallets,
                Categories = Categories
            };
            string json = JsonSerializer.Serialize(data, _options);

            string tempPath = _filePath + ".tmp";
            string backupPath = _filePath + ".bak";

            await File.WriteAllTextAsync(tempPath, json, cancellationToken);

            if (File.Exists(_filePath))
            {
                File.Replace(tempPath, _filePath, backupPath);
            }
            else
            {
                File.Move(tempPath, _filePath);
            }

            _logger.LogInformation("Database saved successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, "FATAL ERROR: Failed to save database changes to '{FilePath}'!", _filePath);
            throw;
        }
        finally
        {
            _semaphore.Release();
        }
    }
}
