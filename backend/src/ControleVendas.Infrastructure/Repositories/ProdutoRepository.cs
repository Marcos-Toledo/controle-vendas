using ControleVendas.Application.Interfaces;
using ControleVendas.Domain.Entities;
using ControleVendas.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ControleVendas.Infrastructure.Repositories;

public class ProdutoRepository(ControleVendasDbContext context) : IProdutoRepository
{
    public Task<Produto?> ObterPorIdAsync(long id, CancellationToken ct = default)
    {
        return context.Produtos.FirstOrDefaultAsync(p => p.Id == id, ct);
    }

    public Task<Produto?> ObterPorNomeAsync(string nome, CancellationToken ct = default)
    {
        return context.Produtos.FirstOrDefaultAsync(
            p => EF.Functions.ILike(p.Nome, nome), ct);
    }

    public async Task<IReadOnlyList<Produto>> ListarAsync(CancellationToken ct = default)
    {
        return await context.Produtos
            .AsNoTracking()
            .OrderBy(p => p.Nome)
            .ToListAsync(ct);
    }

    public async Task AdicionarAsync(Produto produto, CancellationToken ct = default)
    {
        await context.Produtos.AddAsync(produto, ct);
    }

    public void Atualizar(Produto produto)
    {
        context.Produtos.Update(produto);
    }

    public void Remover(Produto produto)
    {
        context.Produtos.Remove(produto);
    }
}
