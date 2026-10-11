using SalesControl.Domain.Entities;

namespace SalesControl.Application.Interfaces;

public interface ISaleRepository
{
    Task<Sale?> GetByIdAsync(long id, CancellationToken ct = default);
    Task<IReadOnlyList<Sale>> ListAsync(CancellationToken ct = default);
    Task AddAsync(Sale sale, CancellationToken ct = default);
}
