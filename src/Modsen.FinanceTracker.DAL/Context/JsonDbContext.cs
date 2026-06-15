using System.Text.Json;
using Modsen.FinanceTracker.DAL.Models;
using Modsen.FinanceTracker.Domain.Entities;
using Modsen.FinanceTracker.Domain.Interfaces;

namespace Modsen.FinanceTracker.DAL.Context;

public sealed class JsonDbContext : IDataContext
{
    private readonly string _filePath;
    private readonly JsonSerializerOptions _options;

    public List<Wallet> Wallets { get; private set; } = [];
    public List<Category> Categories { get; private set; } = [];

    public JsonDbContext(string filePath)
    {
        _filePath = filePath;
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
            return;
        }

        string json = await File.ReadAllTextAsync(_filePath, cancellationToken);
        JsonDataModel? data = JsonSerializer.Deserialize<JsonDataModel>(json, _options);

        if (data is not null)
        {
            Wallets = data.Wallets;
            Categories = data.Categories;
        }
    }
    
    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var data = new JsonDataModel
        {
            Wallets = Wallets, 
            Categories = Categories
        };
        string json = JsonSerializer.Serialize(data, _options);

        await File.WriteAllTextAsync(_filePath, json, cancellationToken);
    }
}