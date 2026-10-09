using ControleVendas.Application.Interfaces;
using ControleVendas.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace ControleVendas.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IClienteService, ClienteService>();
        services.AddScoped<IProdutoService, ProdutoService>();
        services.AddScoped<IVendaService, VendaService>();

        return services;
    }
}
