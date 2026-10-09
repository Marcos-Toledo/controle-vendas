using ControleVendas.Domain.Entities;

namespace ControleVendas.Application.Interfaces;

public interface IVendaRepository
{
    Task<Venda?> ObterPorIdAsync(long id, CancellationToken ct = default);
    Task<IReadOnlyList<Venda>> ListarAsync(CancellationToken ct = default);
    Task AdicionarAsync(Venda venda, CancellationToken ct = default);
}
