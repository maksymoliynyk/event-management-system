using Domain.Aggregates.Events;

namespace Domain.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IEventRepository Event { get; }
    Task SaveAsync(CancellationToken ct);
}