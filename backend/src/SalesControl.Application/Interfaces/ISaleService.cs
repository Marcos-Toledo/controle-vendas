using SalesControl.Application.DTOs;

namespace SalesControl.Application.Interfaces;

public interface ISaleService
{
    Task<IReadOnlyList<SaleResponse>> ListAsync(CancellationToken ct = default);
    Task<SaleResponse> GetByIdAsync(long id, CancellationToken ct = default);
    Task<SaleResponse> CreateAsync(SaleRequest request, CancellationToken ct = default);
}
