using System.Text.Json;
using Modsen.FinanceTracker.DAL.Models;
using Modsen.FinanceTracker.Domain.Entities;
using Modsen.FinanceTracker.Domain.Interfaces;

namespace Modsen.FinanceTracker.DAL.Context;

public sealed class JsonDbContext : IDataContext
{
    private readonly string _filePath;
    private readonly JsonSerializerOptions _options;

    public List<Transaction> Transactions { get; private set; } = [];
    public List<Category> Categories { get; private set; } = [];

    public JsonDbContext(string filePath)
    {
        _filePath = filePath;
        _options = new JsonSerializerOptions { WriteIndented = true };
    }

    public async Task LoadAsync(CancellationToken cancellationToken = default)
    {
        if (!File.Exists(_filePath))
        {
            return;
        }

        var json = await File.ReadAllTextAsync(_filePath, cancellationToken);
        var data = JsonSerializer.Deserialize<JsonDataModel>(json, _options);

        if (data is not null)
        {
            Transactions = data.Transactions;
            Categories = data.Categories;
        }
    }
    
    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var data = new JsonDataModel
        {
            Transactions = Transactions, 
            Categories = Categories
        };
        var json = JsonSerializer.Serialize(data, _options);

        await File.WriteAllTextAsync(_filePath, json, cancellationToken);
    }
}