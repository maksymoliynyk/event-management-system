using System.Data;

using Dapper;

using Domain.Aggregates.Events;
using Domain.Enums;
using Domain.Interfaces;

using Infrastructure;
using Infrastructure.Options;

using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

using static Infrastructure.Constants.DbContextConstants;

namespace IntegrationTests.ApiTests;

public class TestHelpingService : IDisposable
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ConnectionStringsOption _connectionStrings;

    public TestHelpingService()
    {
        var helper = new ConnectionStringHelper();
        _connectionStrings = helper.Options;
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

        // SQL query
        var sql = $"""
                           INSERT INTO {DefaultSchema}.Events 
                           (Id, Title, Description, StartDate, Duration, Location, Status, CreateDate, OwnerId) 
                           VALUES 
                           (@Id, @Title, @Description, @StartDate, @Duration, @Location, @Status, @CreateDate, @OwnerId);
                   """;

        var parameters = new DynamicParameters();
        parameters.Add("@Id", eventId, DbType.Guid);
        parameters.Add("@Title", title, DbType.String);
        parameters.Add("@Description", description, DbType.String);
        parameters.Add("@StartDate", startDate, DbType.DateTime2);
        parameters.Add("@Duration", duration, DbType.Time);
        parameters.Add("@Location", location, DbType.String);
        parameters.Add("@Status", status, DbType.Int16);
        parameters.Add("@CreateDate", createDate, DbType.DateTime2);
        parameters.Add("@OwnerId", userId, DbType.Guid);

        await using var connection = new SqlConnection(_connectionStrings.DefaultConnection);
        await connection.ExecuteAsync(sql, parameters);
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