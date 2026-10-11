using SalesControl.Application.DTOs;
using SalesControl.Application.Exceptions;
using SalesControl.Application.Interfaces;
using SalesControl.Domain.Entities;

namespace SalesControl.Application.Services;

public class SaleService(
    ISaleRepository sales,
    ICustomerRepository customers,
    IProductRepository products,
    IUnitOfWork uow) : ISaleService
{
    public async Task<IReadOnlyList<SaleResponse>> ListAsync(CancellationToken ct = default)
    {
        var list = await sales.ListAsync(ct);
        return list.Select(ToResponse).ToList();
    }

    public async Task<SaleResponse> GetByIdAsync(long id, CancellationToken ct = default)
    {
        var sale = await sales.GetByIdAsync(id, ct)
            ?? throw new NotFoundException("Venda não encontrada.");

        return ToResponse(sale);
    }

    public async Task<SaleResponse> CreateAsync(SaleRequest request, CancellationToken ct = default)
    {
        var customer = await customers.GetByIdAsync(request.CustomerId, ct)
            ?? throw new NotFoundException("Cliente não encontrado.");

        if (request.Items is not { Count: > 0 })
            throw new ValidationException("A venda deve conter pelo menos um produto.");

        var sale = new Sale
        {
            CustomerId = customer.Id,
            SaleDate = request.SaleDate ?? DateOnly.FromDateTime(DateTime.Today)
        };

        foreach (var group in request.Items.GroupBy(i => i.ProductId))
        {
            var product = await products.GetByIdAsync(group.Key, ct)
                ?? throw new ValidationException($"Produto de código {group.Key} não encontrado.");

            var quantity = group.Sum(i => i.Quantity);
            if (quantity <= 0)
                throw new ValidationException(
                    $"A quantidade do produto \"{product.Name}\" deve ser maior que zero.");

            sale.Items.Add(new SaleItem
            {
                ProductId = product.Id,
                Quantity = quantity,
                UnitPrice = product.UnitPrice
            });
        }

        sale.RecalculateTotal();

        await sales.AddAsync(sale, ct);
        await uow.SaveAsync(ct);

        var saved = await sales.GetByIdAsync(sale.Id, ct);
        return ToResponse(saved!);
    }

    private static SaleResponse ToResponse(Sale s)
    {
        var items = s.Items
            .Select(i => new SaleItemResponse(
                i.ProductId,
                i.Product?.Name ?? string.Empty,
                i.Quantity,
                i.UnitPrice,
                i.ItemTotal))
            .ToList();

        return new SaleResponse(
            s.Id,
            s.CustomerId,
            s.Customer?.Name ?? string.Empty,
            s.Customer?.Cpf ?? string.Empty,
            s.SaleDate,
            s.TotalAmount,
            items);
    }
}
