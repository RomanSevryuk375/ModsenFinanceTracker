namespace Modsen.FinanceTracker.Domain.Entities;

public abstract class Transaction
{
    public Guid Id { get; set; }
    public decimal Amount { get; set; }
    public string Description { get; set; }
    public DateTime Date { get; set; }
    public Guid CategoryId { get; set; }

    protected Transaction(Guid id, decimal amount, string description, DateTime date, Guid categoryId)
    {
        Id = id;
        Amount = amount;
        Description = description;
        Date = date;
        CategoryId = categoryId;
    }
}