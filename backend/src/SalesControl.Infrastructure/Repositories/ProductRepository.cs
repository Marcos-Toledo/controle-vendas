using Microsoft.EntityFrameworkCore;
using SalesControl.Application.Interfaces;
using SalesControl.Domain.Entities;
using SalesControl.Infrastructure.Persistence;

namespace SalesControl.Infrastructure.Repositories;

public class ProductRepository(SalesControlDbContext context) : IProductRepository
{
    public Task<Product?> GetByIdAsync(long id, CancellationToken ct = default)
    {
        return context.Products.FirstOrDefaultAsync(p => p.Id == id, ct);
    }

    public Task<Product?> GetByNameAsync(string name, CancellationToken ct = default)
    {
        return context.Products.FirstOrDefaultAsync(
            p => EF.Functions.ILike(p.Name, name), ct);
    }

    public async Task<IReadOnlyList<Product>> ListAsync(CancellationToken ct = default)
    {
        return await context.Products
            .AsNoTracking()
            .OrderBy(p => p.Name)
            .ToListAsync(ct);
    }

    public async Task AddAsync(Product product, CancellationToken ct = default)
    {
        await context.Products.AddAsync(product, ct);
    }

    public void Update(Product product)
    {
        context.Products.Update(product);
    }

    public void Remove(Product product)
    {
        context.Products.Remove(product);
    }
}
