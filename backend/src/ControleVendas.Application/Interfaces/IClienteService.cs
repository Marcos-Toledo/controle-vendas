using ControleVendas.Application.DTOs;

namespace ControleVendas.Application.Interfaces;

public interface IClienteService
{
    Task<IReadOnlyList<ClienteResponse>> ListarAsync(CancellationToken ct = default);
    Task<IReadOnlyList<ClienteResponse>> BuscarAsync(string termo, CancellationToken ct = default);
    Task<ClienteResponse> ObterPorIdAsync(long id, CancellationToken ct = default);
    Task<ClienteResponse> CriarAsync(ClienteRequest request, CancellationToken ct = default);
    Task<ClienteResponse> AtualizarAsync(long id, ClienteRequest request, CancellationToken ct = default);
    Task RemoverAsync(long id, CancellationToken ct = default);
}
