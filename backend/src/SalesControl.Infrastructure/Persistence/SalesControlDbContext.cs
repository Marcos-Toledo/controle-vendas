using Microsoft.EntityFrameworkCore;
using SalesControl.Application.Interfaces;
using SalesControl.Domain.Entities;

namespace SalesControl.Infrastructure.Persistence;

public class SalesControlDbContext : DbContext, IUnitOfWork
{
    public SalesControlDbContext(DbContextOptions<SalesControlDbContext> options)
        : base(options)
    {
    }

    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Sale> Sales => Set<Sale>();
    public DbSet<SaleItem> SaleItems => Set<SaleItem>();

    public Task<int> SaveAsync(CancellationToken ct = default)
    {
        return SaveChangesAsync(ct);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(SalesControlDbContext).Assembly);
    }
}
