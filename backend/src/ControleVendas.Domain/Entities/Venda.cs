namespace ControleVendas.Domain.Entities;

public class Venda
{
    public long Id { get; set; }
    public long ClienteId { get; set; }
    public Cliente Cliente { get; set; } = null!;
    public DateOnly DataVenda { get; set; }
    public decimal ValorTotal { get; private set; }
    public ICollection<ItemVenda> Itens { get; } = new List<ItemVenda>();
    public DateTime CriadoEm { get; set; }

    public void RecalcularTotal()
    {
        ValorTotal = Itens.Sum(item => item.TotalItem);
    }
}
