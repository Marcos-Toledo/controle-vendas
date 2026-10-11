using Microsoft.EntityFrameworkCore;
using SalesControl.Application.Interfaces;
using SalesControl.Domain.Entities;
using SalesControl.Infrastructure.Persistence;

namespace SalesControl.Infrastructure.Repositories;

public class SaleRepository(SalesControlDbContext context) : ISaleRepository
{
    public Task<Sale?> GetByIdAsync(long id, CancellationToken ct = default)
    {
        return context.Sales
            .Include(s => s.Customer)
            .Include(s => s.Items)
                .ThenInclude(i => i.Product)
            .FirstOrDefaultAsync(s => s.Id == id, ct);
    }

    public async Task<IReadOnlyList<Sale>> ListAsync(CancellationToken ct = default)
    {
        return await context.Sales
            .AsNoTracking()
            .Include(s => s.Customer)
            .Include(s => s.Items)
                .ThenInclude(i => i.Product)
            .OrderByDescending(s => s.SaleDate)
            .ThenByDescending(s => s.Id)
            .ToListAsync(ct);
    }

    public async Task AddAsync(Sale sale, CancellationToken ct = default)
    {
        await context.Sales.AddAsync(sale, ct);
    }
}
