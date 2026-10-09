namespace ControleVendas.Domain.Entities;

public class Produto
{
    public long Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public decimal ValorUnitario { get; set; }
    public DateTime CriadoEm { get; set; }
    public DateTime AtualizadoEm { get; set; }
}
