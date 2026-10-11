using SalesControl.Application.DTOs;
using SalesControl.Application.Exceptions;
using SalesControl.Application.Interfaces;
using SalesControl.Application.Services;
using SalesControl.Domain.Entities;

namespace SalesControl.Tests.Application;

public class SaleServiceTests
{
    private readonly List<Customer> customers =
    [
        new() { Id = 1, Cpf = "52998224725", Name = "Maria Silva" }
    ];

    private readonly List<Product> products =
    [
        new() { Id = 1, Name = "Teclado", UnitPrice = 100m },
        new() { Id = 2, Name = "Mouse", UnitPrice = 50m }
    ];

    private SaleService CreateService()
    {
        return new SaleService(
            new FakeSaleRepository(customers, products),
            new FakeCustomerRepository(customers),
            new FakeProductRepository(products),
            new FakeUnitOfWork());
    }

    [Fact]
    public async Task CreateAsync_UnknownCustomer_ThrowsNotFound()
    {
        var request = new SaleRequest(99, null,
            [new SaleItemRequest(1, 1)]);

        await Assert.ThrowsAsync<NotFoundException>(
            () => CreateService().CreateAsync(request));
    }

    [Fact]
    public async Task CreateAsync_WithoutItems_ThrowsValidation()
    {
        var service = CreateService();

        await Assert.ThrowsAsync<ValidationException>(
            () => service.CreateAsync(new SaleRequest(1, null, null)));
        await Assert.ThrowsAsync<ValidationException>(
            () => service.CreateAsync(new SaleRequest(1, null, [])));
    }

    [Fact]
    public async Task CreateAsync_UnknownProduct_ThrowsValidation()
    {
        var request = new SaleRequest(1, null,
            [new SaleItemRequest(99, 1)]);

        await Assert.ThrowsAsync<ValidationException>(
            () => CreateService().CreateAsync(request));
    }

    [Fact]
    public async Task CreateAsync_ZeroQuantity_ThrowsValidation()
    {
        var request = new SaleRequest(1, null,
            [new SaleItemRequest(1, 0)]);

        await Assert.ThrowsAsync<ValidationException>(
            () => CreateService().CreateAsync(request));
    }

    [Fact]
    public async Task CreateAsync_RepeatedProduct_MergesQuantities()
    {
        var request = new SaleRequest(1, null,
        [
            new SaleItemRequest(1, 2),
            new SaleItemRequest(1, 3),
            new SaleItemRequest(2, 1)
        ]);

        var sale = await CreateService().CreateAsync(request);

        Assert.Equal(2, sale.Items.Count);
        var merged = sale.Items.Single(i => i.ProductId == 1);
        Assert.Equal(5, merged.Quantity);
        Assert.Equal(500m, merged.ItemTotal);
    }

    [Fact]
    public async Task CreateAsync_SnapshotsUnitPrice_AndComputesTotal()
    {
        var request = new SaleRequest(1, null,
        [
            new SaleItemRequest(1, 2),
            new SaleItemRequest(2, 1)
        ]);

        var sale = await CreateService().CreateAsync(request);

        Assert.All(sale.Items, i =>
            Assert.Equal(products.Single(p => p.Id == i.ProductId).UnitPrice, i.UnitPrice));
        Assert.Equal(250m, sale.TotalAmount);
    }

    [Fact]
    public async Task CreateAsync_WithoutSaleDate_DefaultsToToday()
    {
        var sale = await CreateService().CreateAsync(
            new SaleRequest(1, null, [new SaleItemRequest(1, 1)]));

        Assert.Equal(DateOnly.FromDateTime(DateTime.Today), sale.SaleDate);
    }

    [Fact]
    public async Task CreateAsync_WithSaleDate_UsesProvidedDate()
    {
        var date = new DateOnly(2026, 1, 15);

        var sale = await CreateService().CreateAsync(
            new SaleRequest(1, date, [new SaleItemRequest(1, 1)]));

        Assert.Equal(date, sale.SaleDate);
    }

    private sealed class FakeCustomerRepository(List<Customer> seed) : ICustomerRepository
    {
        public Task<Customer?> GetByIdAsync(long id, CancellationToken ct = default)
            => Task.FromResult(seed.FirstOrDefault(c => c.Id == id));

        public Task<Customer?> GetByCpfAsync(string cpf, CancellationToken ct = default)
            => Task.FromResult(seed.FirstOrDefault(c => c.Cpf == cpf));

        public Task<IReadOnlyList<Customer>> ListAsync(CancellationToken ct = default)
            => Task.FromResult<IReadOnlyList<Customer>>(seed);

        public Task<IReadOnlyList<Customer>> SearchByNameOrCpfAsync(string term, CancellationToken ct = default)
            => Task.FromResult<IReadOnlyList<Customer>>(seed);

        public Task AddAsync(Customer customer, CancellationToken ct = default)
        {
            seed.Add(customer);
            return Task.CompletedTask;
        }

        public void Update(Customer customer) { }

        public void Remove(Customer customer) => seed.Remove(customer);
    }

    private sealed class FakeProductRepository(List<Product> seed) : IProductRepository
    {
        public Task<Product?> GetByIdAsync(long id, CancellationToken ct = default)
            => Task.FromResult(seed.FirstOrDefault(p => p.Id == id));

        public Task<Product?> GetByNameAsync(string name, CancellationToken ct = default)
            => Task.FromResult(seed.FirstOrDefault(p => p.Name == name));

        public Task<IReadOnlyList<Product>> ListAsync(CancellationToken ct = default)
            => Task.FromResult<IReadOnlyList<Product>>(seed);

        public Task AddAsync(Product product, CancellationToken ct = default)
        {
            seed.Add(product);
            return Task.CompletedTask;
        }

        public void Update(Product product) { }

        public void Remove(Product product) => seed.Remove(product);
    }

    private sealed class FakeSaleRepository(
        List<Customer> customers, List<Product> products) : ISaleRepository
    {
        private readonly List<Sale> sales = [];

        public Task<Sale?> GetByIdAsync(long id, CancellationToken ct = default)
        {
            var sale = sales.FirstOrDefault(s => s.Id == id);
            if (sale is not null)
            {
                sale.Customer = customers.First(c => c.Id == sale.CustomerId);
                foreach (var item in sale.Items)
                    item.Product = products.First(p => p.Id == item.ProductId);
            }
            return Task.FromResult(sale);
        }

        public Task<IReadOnlyList<Sale>> ListAsync(CancellationToken ct = default)
            => Task.FromResult<IReadOnlyList<Sale>>(sales);

        public Task AddAsync(Sale sale, CancellationToken ct = default)
        {
            sale.Id = sales.Count + 1;
            sales.Add(sale);
            return Task.CompletedTask;
        }
    }

    private sealed class FakeUnitOfWork : IUnitOfWork
    {
        public Task<int> SaveAsync(CancellationToken ct = default)
            => Task.FromResult(0);
    }
}
