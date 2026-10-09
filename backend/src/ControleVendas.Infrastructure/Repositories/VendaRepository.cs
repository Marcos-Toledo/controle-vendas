using ControleVendas.Application.Interfaces;
using ControleVendas.Domain.Entities;
using ControleVendas.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ControleVendas.Infrastructure.Repositories;

public class VendaRepository(ControleVendasDbContext context) : IVendaRepository
{
    public Task<Venda?> ObterPorIdAsync(long id, CancellationToken ct = default)
    {
        return context.Vendas
            .Include(v => v.Cliente)
            .Include(v => v.Itens)
                .ThenInclude(i => i.Produto)
            .FirstOrDefaultAsync(v => v.Id == id, ct);
    }

    public async Task<IReadOnlyList<Venda>> ListarAsync(CancellationToken ct = default)
    {
        return await context.Vendas
            .AsNoTracking()
            .Include(v => v.Cliente)
            .Include(v => v.Itens)
                .ThenInclude(i => i.Produto)
            .OrderByDescending(v => v.DataVenda)
            .ThenByDescending(v => v.Id)
            .ToListAsync(ct);
    }

    public async Task AdicionarAsync(Venda venda, CancellationToken ct = default)
    {
        await context.Vendas.AddAsync(venda, ct);
    }
}
