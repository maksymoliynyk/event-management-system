using System;
using System.Threading;
using System.Threading.Tasks;

using Domain.Aggregates.Events;

namespace Domain.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IEventRepository Event { get; }
    Task SaveAsync(CancellationToken ct);
}