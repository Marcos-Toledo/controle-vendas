using ControleVendas.Application.Interfaces;
using ControleVendas.Domain.Entities;
using ControleVendas.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ControleVendas.Infrastructure.Repositories;

public class ClienteRepository(ControleVendasDbContext context) : IClienteRepository
{
    public Task<Cliente?> ObterPorIdAsync(long id, CancellationToken ct = default)
    {
        return context.Clientes.FirstOrDefaultAsync(c => c.Id == id, ct);
    }

    public Task<Cliente?> ObterPorCpfAsync(string cpf, CancellationToken ct = default)
    {
        return context.Clientes.FirstOrDefaultAsync(c => c.Cpf == cpf, ct);
    }

    public async Task<IReadOnlyList<Cliente>> ListarAsync(CancellationToken ct = default)
    {
        return await context.Clientes
            .AsNoTracking()
            .OrderBy(c => c.Nome)
            .ToListAsync(ct);
    }

    public async Task<IReadOnlyList<Cliente>> BuscarPorNomeOuCpfAsync(string termo, CancellationToken ct = default)
    {
        var digitos = new string(termo.Where(char.IsDigit).ToArray());

        return await context.Clientes
            .AsNoTracking()
            .Where(c => EF.Functions.ILike(c.Nome, $"%{termo}%")
                        || (digitos.Length > 0 && c.Cpf.StartsWith(digitos)))
            .OrderBy(c => c.Nome)
            .Take(20)
            .ToListAsync(ct);
    }

    public async Task AdicionarAsync(Cliente cliente, CancellationToken ct = default)
    {
        await context.Clientes.AddAsync(cliente, ct);
    }

    public void Atualizar(Cliente cliente)
    {
        context.Clientes.Update(cliente);
    }

    public void Remover(Cliente cliente)
    {
        context.Clientes.Remove(cliente);
    }
}
