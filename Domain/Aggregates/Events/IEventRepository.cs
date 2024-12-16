namespace Domain.Aggregates.Events;

public interface IEventRepository
{
    void Create(Event @event);
    /// <summary>
    /// Already filtered by owner for owner-based actions
    /// </summary>
    /// <param name="eventId"></param>
    /// <param name="ownerId"></param>
    /// <returns></returns>
    Event GetById(Guid eventId, Guid ownerId);
    Event GetById(Guid eventId);
    void Delete(Event @event);
    void Update(Event @event);
}