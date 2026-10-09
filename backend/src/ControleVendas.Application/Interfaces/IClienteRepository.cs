using ControleVendas.Domain.Entities;

namespace ControleVendas.Application.Interfaces;

public interface IClienteRepository
{
    Task<Cliente?> ObterPorIdAsync(long id, CancellationToken ct = default);
    Task<Cliente?> ObterPorCpfAsync(string cpf, CancellationToken ct = default);
    Task<IReadOnlyList<Cliente>> ListarAsync(CancellationToken ct = default);
    Task<IReadOnlyList<Cliente>> BuscarPorNomeOuCpfAsync(string termo, CancellationToken ct = default);
    Task AdicionarAsync(Cliente cliente, CancellationToken ct = default);
    void Atualizar(Cliente cliente);
    void Remover(Cliente cliente);
}
