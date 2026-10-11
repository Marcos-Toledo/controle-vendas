using SalesControl.Domain.Entities;

namespace SalesControl.Application.Interfaces;

public interface ICustomerRepository
{
    Task<Customer?> GetByIdAsync(long id, CancellationToken ct = default);
    Task<Customer?> GetByCpfAsync(string cpf, CancellationToken ct = default);
    Task<IReadOnlyList<Customer>> ListAsync(CancellationToken ct = default);
    Task<IReadOnlyList<Customer>> SearchByNameOrCpfAsync(string term, CancellationToken ct = default);
    Task AddAsync(Customer customer, CancellationToken ct = default);
    void Update(Customer customer);
    void Remove(Customer customer);
}
