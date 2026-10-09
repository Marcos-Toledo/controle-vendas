using ControleVendas.Application.DTOs;

namespace ControleVendas.Application.Interfaces;

public interface IProdutoService
{
    Task<IReadOnlyList<ProdutoResponse>> ListarAsync(CancellationToken ct = default);
    Task<ProdutoResponse> ObterPorIdAsync(long id, CancellationToken ct = default);
    Task<ProdutoResponse> CriarAsync(ProdutoRequest request, CancellationToken ct = default);
    Task<ProdutoResponse> AtualizarAsync(long id, ProdutoRequest request, CancellationToken ct = default);
    Task RemoverAsync(long id, CancellationToken ct = default);
}
