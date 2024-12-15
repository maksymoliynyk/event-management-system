using System;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

using Application.Enums;
using Application.Interfaces.Repositories;
using Application.Models;

using Dapper;

using Domain.Exceptions;

using static Infrastructure.Constants.DbContextConstants;

namespace Infrastructure.Repositories;

public class EventQueryRepository : IEventQueryRepository
{
    private readonly IDbConnection _dbConnection;

    public EventQueryRepository(IDbConnection dbConnection)
    {
        _dbConnection = dbConnection;
    }

    public async Task<IEnumerable<EventQueryModel>> GetAllEventsForUser(Guid userId, EventQueryingMode mode,
        CancellationToken ct)
    {
        var whereClause = mode switch
        {
            EventQueryingMode.Owner => @"WHERE e.OwnerId = @UserId",
            EventQueryingMode.Attendee => $@"
                 JOIN {DefaultSchema}.Attendees a ON e.Id = a.EventId
                WHERE a.AttendeeId = @UserId",
            _ => throw new ArgumentOutOfRangeException(nameof(mode), "Invalid querying mode")
        };

        var query = $@"SELECT e.Id, e.Title, e.Description, e.StartDate, e.Duration, 
                       e.Location, e.Status, e.CreateDate
                FROM {DefaultSchema}.Events e
                {whereClause}";

        return await _dbConnection.QueryAsync<EventQueryModel>(query,
            new { UserId = userId });
    }

    public async Task<EventQueryModel> GetEventById(Guid eventId, Guid userId, CancellationToken ct)
    {
        var sql = $@"
        SELECT e.Id, e.Title, e.Description, e.StartDate, e.Duration, 
               e.Location, e.Status, e.CreateDate
        FROM {DefaultSchema}.Events e
        LEFT JOIN {DefaultSchema}.RSVPs r ON e.Id = r.EventId AND r.UserId = @UserId
        WHERE e.Id = @EventId AND (e.OwnerId = @UserId OR r.UserId = @UserId)";

        var result = await _dbConnection.QueryFirstOrDefaultAsync<EventQueryModel>(sql,
            new { EventId = eventId, UserId = userId });
        if (result == null)
        {
            throw new ObjectNotFoundException(EntitiesErrorType.Event);
        }

        return result;
    }

    public async Task<IEnumerable<AttendeeQueryModel>> GetAllAttendeesByEvent(Guid eventId, Guid ownerId,
        CancellationToken ct)
    {
        var sql = $@"
            SELECT u.Email AS AttendeeEmail, a.EventId
            FROM {DefaultSchema}.Attendees a
                JOIN {DefaultSchema}.Events e on a.EventId = e.Id
                JOIN {DefaultSchema}.AspNetUsers u ON a.AttendeeId = u.Id
            WHERE a.EventId = @eventId and e.OwnerId = @OwnerId";

        return await _dbConnection.QueryAsync<AttendeeQueryModel>(sql,
            new { EventId = eventId, OwnerId = ownerId });
    }

    public async Task<IEnumerable<RSVPQueryModel>> GetAllInvitesByEvent(Guid eventId, Guid ownerId,
        CancellationToken ct)
    {
        var sql = $@"
            SELECT u.Email AS InviteeEmail, r.EventId, r.Status AS RSVPStatus
            FROM {DefaultSchema}.RSVPs r
                JOIN {DefaultSchema}.Events e on r.EventId = e.Id
                JOIN {DefaultSchema}.AspNetUsers u ON r.UserId = u.Id
            WHERE r.EventId = @EventId and e.OwnerId = @OwnerId";

        return await _dbConnection.QueryAsync<RSVPQueryModel>(sql,
            new { EventId = eventId, OwnerId = ownerId });
    }

    public async Task<IEnumerable<RSVPQueryModel>> GetAllInvitesByUser(Guid userId, CancellationToken ct)
    {
        var sql = $@"
            SELECT u.Email AS InviteeEmail, r.EventId, r.Status AS RSVPStatus
            FROM {DefaultSchema}.RSVPs r
                JOIN {DefaultSchema}.AspNetUsers u ON r.UserId = u.Id
            WHERE r.UserId = @UserId";

        return await _dbConnection.QueryAsync<RSVPQueryModel>(sql,
            new { UserId = userId });
    }
}