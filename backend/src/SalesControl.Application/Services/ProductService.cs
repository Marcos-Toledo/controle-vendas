using SalesControl.Application.DTOs;
using SalesControl.Application.Exceptions;
using SalesControl.Application.Interfaces;
using SalesControl.Domain.Entities;

namespace SalesControl.Application.Services;

public class ProductService(IProductRepository products, IUnitOfWork uow) : IProductService
{
    public async Task<IReadOnlyList<ProductResponse>> ListAsync(CancellationToken ct = default)
    {
        var list = await products.ListAsync(ct);
        return list.Select(ToResponse).ToList();
    }

    public async Task<ProductResponse> GetByIdAsync(long id, CancellationToken ct = default)
    {
        return ToResponse(await GetOrThrowAsync(id, ct));
    }

    public async Task<ProductResponse> CreateAsync(ProductRequest request, CancellationToken ct = default)
    {
        var product = new Product();
        await ApplyDataAsync(product, request, ct);

        product.CreatedAt = product.UpdatedAt = DateTime.UtcNow;

        await products.AddAsync(product, ct);
        await uow.SaveAsync(ct);

        return ToResponse(product);
    }

    public async Task<ProductResponse> UpdateAsync(long id, ProductRequest request, CancellationToken ct = default)
    {
        var product = await GetOrThrowAsync(id, ct);
        await ApplyDataAsync(product, request, ct);

        product.UpdatedAt = DateTime.UtcNow;

        products.Update(product);
        await uow.SaveAsync(ct);

        return ToResponse(product);
    }

    public async Task DeleteAsync(long id, CancellationToken ct = default)
    {
        var product = await GetOrThrowAsync(id, ct);

        products.Remove(product);
        await uow.SaveAsync(ct);
    }

    private async Task<Product> GetOrThrowAsync(long id, CancellationToken ct)
    {
        return await products.GetByIdAsync(id, ct)
            ?? throw new NotFoundException("Produto não encontrado.");
    }

    private async Task ApplyDataAsync(Product product, ProductRequest request, CancellationToken ct)
    {
        var name = (request.Name ?? string.Empty).Trim();
        if (name.Length == 0)
            throw new ValidationException("O nome é obrigatório.");

        var withSameName = await products.GetByNameAsync(name, ct);
        if (withSameName is not null && withSameName.Id != product.Id)
            throw new ConflictException("Já existe um produto cadastrado com este nome.");

        if (request.UnitPrice < 0)
            throw new ValidationException("O valor unitário deve ser maior ou igual a zero.");

        product.Name = name;
        product.UnitPrice = request.UnitPrice;
    }

    private static ProductResponse ToResponse(Product p)
    {
        return new ProductResponse(p.Id, p.Name, p.UnitPrice);
    }
}
