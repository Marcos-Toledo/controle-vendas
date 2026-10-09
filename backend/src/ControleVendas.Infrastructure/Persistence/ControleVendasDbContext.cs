using ControleVendas.Application.Interfaces;
using ControleVendas.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ControleVendas.Infrastructure.Persistence;

public class ControleVendasDbContext : DbContext, IUnitOfWork
{
    public ControleVendasDbContext(DbContextOptions<ControleVendasDbContext> options)
        : base(options)
    {
    }

    public DbSet<Cliente> Clientes => Set<Cliente>();
    public DbSet<Produto> Produtos => Set<Produto>();
    public DbSet<Venda> Vendas => Set<Venda>();
    public DbSet<ItemVenda> ItensVenda => Set<ItemVenda>();

    public Task<int> SalvarAsync(CancellationToken ct = default)
    {
        return SaveChangesAsync(ct);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ControleVendasDbContext).Assembly);
    }
}
