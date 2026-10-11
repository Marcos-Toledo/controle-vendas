namespace SalesControl.Domain.Entities;

public class SaleItem
{
    private decimal _itemTotal;

    public long Id { get; set; }
    public long SaleId { get; set; }
    public long ProductId { get; set; }
    public Product Product { get; set; } = null!;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }

    public decimal ItemTotal
    {
        get => Quantity * UnitPrice;
        private set => _itemTotal = value;
    }
}
