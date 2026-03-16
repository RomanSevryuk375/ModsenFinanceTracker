namespace Modsen.FinanceTracker.Infrastructure.Configuration;

public class AppConfiguration
{
    private static readonly Lazy<AppConfiguration> _instance = 
        new (() => new AppConfiguration());

    public static AppConfiguration Instance => _instance.Value;

    private AppConfiguration() { } 

    public string JsonDbPath { get; set; } = "data.json";
    public string Currency { get; set; } = "BYN";
    public string DateFormat { get; set; } = "dd.MM.yyyy";
}