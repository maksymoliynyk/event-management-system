using System;
using System.Collections.Generic;

using Application.Models;

namespace Application.Queries.Attendees;

public sealed record GetAllAttendeesByEventQuery(Guid EventId, Guid OwnerId)
    : IRequest<GetAllAttendeesByEventQueryResult>;

public sealed record GetAllAttendeesByEventQueryResult(IEnumerable<AttendeeQueryModel> Attendees);