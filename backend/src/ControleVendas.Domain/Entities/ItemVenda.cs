namespace ControleVendas.Domain.Entities;

public class ItemVenda
{
    private decimal _totalItem;

    public long Id { get; set; }
    public long VendaId { get; set; }
    public long ProdutoId { get; set; }
    public Produto Produto { get; set; } = null!;
    public int Quantidade { get; set; }
    public decimal PrecoUnitario { get; set; }

    public decimal TotalItem
    {
        get => Quantidade * PrecoUnitario;
        private set => _totalItem = value;
    }
}
