namespace ControleVendas.Domain.Entities;

public class ItemVenda
{
    public long Id { get; set; }
    public long VendaId { get; set; }
    public long ProdutoId { get; set; }
    public Produto Produto { get; set; } = null!;
    public int Quantidade { get; set; }
    public decimal PrecoUnitario { get; set; }
    public decimal TotalItem => Quantidade * PrecoUnitario;
}
