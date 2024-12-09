using Domain.Aggregates.Events;
using Domain.Interfaces;

using Infrastructure;

using Microsoft.EntityFrameworkCore;

namespace IntegrationTests.ApiTests;

public class TestHelpingService : IDisposable
{
    private readonly IUnitOfWork _unitOfWork;

    public TestHelpingService()
    {
        var helper = new ConnectionStringHelper();
        var context = new EventManagementContext(new DbContextOptionsBuilder<EventManagementContext>()
            .UseSqlServer(helper.Options.DefaultConnection)
            .Options);
        _unitOfWork = new UnitOfWork(context);
    }

    public async Task<Event> CreateEvent(Guid userId, CancellationToken ct)
    {
        var newEvent = Event.CreateEvent(Company.Name(),
            Lorem.Sentence(10),
            TimeSpan.FromHours(1),
            Address.StreetAddress(),
            userId,
            DateTime.UtcNow.AddDays(1));
        _unitOfWork.Event.Create(newEvent);
        await _unitOfWork.SaveAsync(ct);
        return newEvent;
    }

    public void Dispose()
    {
        _unitOfWork.Dispose();
    }
}