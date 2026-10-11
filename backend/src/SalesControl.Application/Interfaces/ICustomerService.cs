using SalesControl.Application.DTOs;

namespace SalesControl.Application.Interfaces;

public interface ICustomerService
{
    Task<IReadOnlyList<CustomerResponse>> ListAsync(CancellationToken ct = default);
    Task<IReadOnlyList<CustomerResponse>> SearchAsync(string term, CancellationToken ct = default);
    Task<CustomerResponse> GetByIdAsync(long id, CancellationToken ct = default);
    Task<CustomerResponse> CreateAsync(CustomerRequest request, CancellationToken ct = default);
    Task<CustomerResponse> UpdateAsync(long id, CustomerRequest request, CancellationToken ct = default);
    Task DeleteAsync(long id, CancellationToken ct = default);
}
