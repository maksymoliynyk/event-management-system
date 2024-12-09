using Domain.Aggregates.Events;
using Domain.Enums;
using Domain.Interfaces;

using Infrastructure;

using Microsoft.EntityFrameworkCore;

using static Infrastructure.Constants.DbContextConstants;

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

    public async Task<Guid> CreateEventToCorruptData(Guid userId, CancellationToken ct,
        string title = null,
        string description = null,
        TimeSpan? duration = null,
        string location = null,
        DateTime? startDate = null,
        EventStatus? status = null
    )
    {
        await using var context = InitializeContext();
        var eventId = Guid.NewGuid();

        // Default values for null arguments
        title ??= "Default Title";
        description ??= "Default Description";
        duration ??= TimeSpan.FromHours(1);
        location ??= "Default Location";
        startDate ??= DateTime.UtcNow.AddDays(1);
        status ??= EventStatus.InProgress;

        var createDate = DateTime.UtcNow;

        // SQL script
        var sql = @$"
        INSERT INTO {DefaultSchema}.Events 
        (Id, Title, Description, StartDate, Duration, Location, Status, CreateDate, OwnerId) 
        VALUES 
        ({0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8});
    ";

        // Execute SQL script
        await context.Database.ExecuteSqlRawAsync(sql,
            eventId,
            title,
            description,
            startDate,
            duration.Value.TotalSeconds,
            location,
            (byte)status,
            createDate,
            userId);

        return eventId;
    }

    public void Dispose()
    {
        _unitOfWork.Dispose();
    }

    private static EventManagementContext InitializeContext()
    {
        var helper = new ConnectionStringHelper();
        return new EventManagementContext(new DbContextOptionsBuilder<EventManagementContext>()
            .UseSqlServer(helper.Options.DefaultConnection)
            .Options);
    }
}