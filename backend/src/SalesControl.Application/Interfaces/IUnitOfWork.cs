namespace SalesControl.Application.Interfaces;

public interface IUnitOfWork
{
    Task<int> SaveAsync(CancellationToken ct = default);
}
