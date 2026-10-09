using ControleVendas.Domain.Entities;

namespace ControleVendas.Application.Interfaces;

public interface IProdutoRepository
{
    Task<Produto?> ObterPorIdAsync(long id, CancellationToken ct = default);
    Task<Produto?> ObterPorNomeAsync(string nome, CancellationToken ct = default);
    Task<IReadOnlyList<Produto>> ListarAsync(CancellationToken ct = default);
    Task AdicionarAsync(Produto produto, CancellationToken ct = default);
    void Atualizar(Produto produto);
    void Remover(Produto produto);
}
