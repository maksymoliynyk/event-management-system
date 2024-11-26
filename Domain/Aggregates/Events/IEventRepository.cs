using System;

namespace Domain.Aggregates.Events;

public interface IEventRepository
{
    void Create(Event @event);
    Event GetById(Guid eventId, Guid userId);
    void Delete(Event @event);
    void Update(Event @event);
}