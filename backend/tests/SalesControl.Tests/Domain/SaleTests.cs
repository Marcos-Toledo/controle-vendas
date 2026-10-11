using SalesControl.Domain.Entities;

namespace SalesControl.Tests.Domain;

public class SaleTests
{
    [Fact]
    public void RecalculateTotal_SumsItemsTotal()
    {
        var sale = new Sale();

        sale.Items.Add(new SaleItem { ProductId = 1, Quantity = 2, UnitPrice = 10.50m });
        sale.Items.Add(new SaleItem { ProductId = 2, Quantity = 1, UnitPrice = 4.00m });

        sale.RecalculateTotal();

        Assert.Equal(25.00m, sale.TotalAmount);
    }

    [Fact]
    public void RecalculateTotal_WithoutItems_ZeroesTotal()
    {
        var sale = new Sale();

        sale.RecalculateTotal();

        Assert.Equal(0m, sale.TotalAmount);
    }
}
