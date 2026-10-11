using Microsoft.EntityFrameworkCore;
using SalesControl.Application.Interfaces;
using SalesControl.Domain.Entities;
using SalesControl.Infrastructure.Persistence;

namespace SalesControl.Infrastructure.Repositories;

public class CustomerRepository(SalesControlDbContext context) : ICustomerRepository
{
    public Task<Customer?> GetByIdAsync(long id, CancellationToken ct = default)
    {
        return context.Customers.FirstOrDefaultAsync(c => c.Id == id, ct);
    }

    public Task<Customer?> GetByCpfAsync(string cpf, CancellationToken ct = default)
    {
        return context.Customers.FirstOrDefaultAsync(c => c.Cpf == cpf, ct);
    }

    public async Task<IReadOnlyList<Customer>> ListAsync(CancellationToken ct = default)
    {
        return await context.Customers
            .AsNoTracking()
            .OrderBy(c => c.Name)
            .ToListAsync(ct);
    }

    public async Task<IReadOnlyList<Customer>> SearchByNameOrCpfAsync(string term, CancellationToken ct = default)
    {
        var digits = new string(term.Where(char.IsDigit).ToArray());

        return await context.Customers
            .AsNoTracking()
            .Where(c => EF.Functions.ILike(c.Name, $"%{term}%")
                        || (digits.Length > 0 && c.Cpf.StartsWith(digits)))
            .OrderBy(c => c.Name)
            .Take(20)
            .ToListAsync(ct);
    }

    public async Task AddAsync(Customer customer, CancellationToken ct = default)
    {
        await context.Customers.AddAsync(customer, ct);
    }

    public void Update(Customer customer)
    {
        context.Customers.Update(customer);
    }

    public void Remove(Customer customer)
    {
        context.Customers.Remove(customer);
    }
}
