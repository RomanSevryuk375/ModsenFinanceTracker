using System.Text.Json;
using Modsen.FinanceTracker.DAL.Models;
using Modsen.FinanceTracker.Domain.Entities;
using Modsen.FinanceTracker.Domain.Interfaces;

namespace Modsen.FinanceTracker.DAL.Context;

public class JsonDbContext : IDataContext
{
    private readonly string _filePath;
    private readonly JsonSerializerOptions _options;

    public List<Transaction> Transactions { get; private set; } = new();
    public List<Category> Categories { get; private set; } = new();

    public JsonDbContext(string filePath)
    {
        _filePath = filePath;
        _options = new JsonSerializerOptions { WriteIndented = true };
    }

    public async Task LoadAsync(CancellationToken ct = default)
    {
        if (!File.Exists(_filePath))
        {
            return;
        }

        var json = await File.ReadAllTextAsync(_filePath, ct);
        var data = JsonSerializer.Deserialize<JsonDataModel>(json, _options);

        if (data is not null)
        {
            Transactions = data.Transactions;
            Categories = data.Categories;
        }
    }
    
    public async Task SaveChangesAsync(CancellationToken ct = default)
    {
        var data = new JsonDataModel
        {
            Transactions = Transactions, 
            Categories = Categories
        };
        var json = JsonSerializer.Serialize(data, _options);
        await File.WriteAllTextAsync(_filePath, json, ct);
    }
}