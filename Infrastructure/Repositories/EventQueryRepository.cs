using System;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

using Application.Enums;
using Application.Interfaces;
using Application.Queries.Models;

using Dapper;

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
            EventQueryingMode.Attendee => @"
                INNER JOIN eventmanagement.Attendees a ON e.Id = a.EventId
                WHERE a.AttendeeId = @UserId",
            _ => throw new ArgumentOutOfRangeException(nameof(mode), "Invalid querying mode")
        };

        var query = $@"SELECT e.Id, e.Title, e.Description, e.StartDate, e.Duration, 
                       e.Location, e.Status, e.CreateDate
                FROM eventmanagement.Events e
                {whereClause}";

        return await _dbConnection.QueryAsync<EventQueryModel>(query,
            new { UserId = userId });
    }

    public async Task<EventQueryModel> GetEventById(Guid eventId, Guid userId, CancellationToken ct)
    {
        var sql = @"
        SELECT e.Id, e.Title, e.Description, e.StartDate, e.Duration, 
               e.Location, e.Status, e.CreateDate
        FROM eventmanagement.Events e
        LEFT JOIN eventmanagement.Attendees a ON e.Id = a.EventId AND a.AttendeeId = @UserId
        WHERE e.Id = @EventId AND (e.OwnerId = @UserId OR a.AttendeeId = @UserId)";

        return await _dbConnection.QuerySingleOrDefaultAsync<EventQueryModel>(sql,
            new { EventId = eventId, UserId = userId });
    }

    public async Task<IEnumerable<AttendeeQueryModel>> GetAllAttendeesByEvent(Guid eventId, Guid ownerId,
        CancellationToken ct)
    {
        var sql = @"
            SELECT u.Email AS AttendeeEmail, a.EventId
            FROM eventmanagement.Attendees a
            INNER JOIN eventmanagement.AspNetUsers u ON a.AttendeeId = u.Id
            WHERE a.EventId;";

        return await _dbConnection.QueryAsync<AttendeeQueryModel>(sql,
            new { EventId = eventId });
    }

    public async Task<IEnumerable<RSVPQueryModel>> GetAllInvitesByEvent(Guid eventId, Guid ownerId,
        CancellationToken ct)
    {
        var sql = @"
            SELECT u.Email AS InviteeEmail, r.EventId, r.Status AS RSVPStatus
            FROM eventmanagement.RSVPs r
            INNER JOIN eventmanagement.AspNetUsers u ON r.UserId = u.Id
            WHERE r.EventId = @EventId";

        return await _dbConnection.QueryAsync<RSVPQueryModel>(sql,
            new { EventId = eventId });
    }

    public async Task<IEnumerable<RSVPQueryModel>> GetAllInvitesByUser(Guid userId, CancellationToken ct)
    {
        var sql = @"
            SELECT u.Email AS InviteeEmail, r.EventId, r.Status AS RSVPStatus
            FROM eventmanagement.RSVPs r
            INNER JOIN eventmanagement.AspNetUsers u ON r.UserId = u.Id
            WHERE r.UserId = @UserId";

        return await _dbConnection.QueryAsync<RSVPQueryModel>(sql,
            new { UserId = userId });
    }
}