namespace SalesControl.Domain.Entities;

public class Sale
{
    public long Id { get; set; }
    public long CustomerId { get; set; }
    public Customer Customer { get; set; } = null!;
    public DateOnly SaleDate { get; set; }
    public decimal TotalAmount { get; private set; }
    public ICollection<SaleItem> Items { get; } = new List<SaleItem>();
    public DateTime CreatedAt { get; set; }

    public void RecalculateTotal()
    {
        TotalAmount = Items.Sum(item => item.ItemTotal);
    }
}
