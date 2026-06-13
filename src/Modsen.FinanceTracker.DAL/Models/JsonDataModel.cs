using Modsen.FinanceTracker.Domain.Entities;

namespace Modsen.FinanceTracker.DAL.Models;

public sealed record JsonDataModel
{
    public List<Transaction> Transactions { get; set; } = [];
    public List<Category> Categories { get; set; } = [];
}