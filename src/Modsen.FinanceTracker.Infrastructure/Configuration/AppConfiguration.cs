namespace Modsen.FinanceTracker.Infrastructure.Configuration;

public sealed class AppConfiguration
{
    private static readonly Lazy<AppConfiguration> _instance = 
        new (() => new AppConfiguration());

    public static AppConfiguration Instance => _instance.Value;

    private AppConfiguration() { } 

    public string JsonDbPath { get; set; } = "data.json";
    public string Currency { get; set; } = "BYN";
    public string DateFormat { get; set; } = "dd.MM.yyyy";
    public bool IsPasswordEnabled { get; set; } = true;
    public string HashedPassword { get; set; } = "240be518fabd2724ddb6f04eeb1da5967448d7e831c08c8fa822809f74c720a9";
}