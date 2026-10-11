using SalesControl.Application.DTOs;

namespace SalesControl.Application.Interfaces;

public interface IProductService
{
    Task<IReadOnlyList<ProductResponse>> ListAsync(CancellationToken ct = default);
    Task<ProductResponse> GetByIdAsync(long id, CancellationToken ct = default);
    Task<ProductResponse> CreateAsync(ProductRequest request, CancellationToken ct = default);
    Task<ProductResponse> UpdateAsync(long id, ProductRequest request, CancellationToken ct = default);
    Task DeleteAsync(long id, CancellationToken ct = default);
}
