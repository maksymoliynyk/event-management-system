﻿using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Application.Enums;
using Application.Queries.Models;

namespace Application.Interfaces;

public interface IEventQueryRepository
{
    Task<IEnumerable<EventQueryModel>> GetAllEventsForUser(Guid userId, EventQueryingMode mode, CancellationToken ct);
    Task<EventQueryModel> GetEventById(Guid eventId, Guid userId, CancellationToken ct);
    Task<IEnumerable<AttendeeQueryModel>> GetAllAttendeesByEvent(Guid eventId, Guid ownerId, CancellationToken ct);
    Task<IEnumerable<RSVPQueryModel>> GetAllInvitesByEvent(Guid eventId, Guid ownerId, CancellationToken ct);
    Task<IEnumerable<RSVPQueryModel>> GetAllInvitesByUser(Guid userId, CancellationToken ct);
}