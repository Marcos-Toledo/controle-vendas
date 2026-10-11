using Microsoft.Extensions.DependencyInjection;
using SalesControl.Application.Interfaces;
using SalesControl.Application.Services;

namespace SalesControl.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<ICustomerService, CustomerService>();
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<ISaleService, SaleService>();

        return services;
    }
}
