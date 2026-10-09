namespace ControleVendas.Domain.Entities;

public class Cliente
{
    public long Id { get; set; }
    public string Cpf { get; set; } = string.Empty;
    public string Nome { get; set; } = string.Empty;
    public DateOnly? DataNascimento { get; set; }
    public string? Email { get; set; }
    public string? Telefone { get; set; }
    public DateTime CriadoEm { get; set; }
    public DateTime AtualizadoEm { get; set; }
}
