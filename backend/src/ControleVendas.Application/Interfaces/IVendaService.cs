using ControleVendas.Application.DTOs;

namespace ControleVendas.Application.Interfaces;

public interface IVendaService
{
    Task<IReadOnlyList<VendaResponse>> ListarAsync(CancellationToken ct = default);
    Task<VendaResponse> ObterPorIdAsync(long id, CancellationToken ct = default);
    Task<VendaResponse> CriarAsync(VendaRequest request, CancellationToken ct = default);
}
