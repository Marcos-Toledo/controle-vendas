namespace ControleVendas.Application.Interfaces;

public interface IUnitOfWork
{
    Task<int> SalvarAsync(CancellationToken ct = default);
}
