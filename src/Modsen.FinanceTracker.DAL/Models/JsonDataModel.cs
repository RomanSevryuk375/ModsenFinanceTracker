using Modsen.FinanceTracker.Domain.Entities;

namespace Modsen.FinanceTracker.DAL.Models;

internal class JsonDataModel
{
    public List<Transaction> Transactions { get; set; } = new();
    public List<Category> Categories { get; set; } = new();
}