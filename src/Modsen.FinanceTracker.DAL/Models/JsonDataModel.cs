namespace Modsen.FinanceTracker.DAL.Models;

public sealed record JsonDataModel
{
    public List<Wallet> Wallets { get; set; } = [];
    public List<Category> Categories { get; set; } = [];
}
