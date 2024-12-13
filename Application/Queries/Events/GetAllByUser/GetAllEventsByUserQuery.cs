using System;
using System.Collections.Generic;

using Application.Enums;
using Application.Models;

namespace Application.Queries.Events;

public sealed record GetAllEventsByUserQuery(Guid UserId, EventQueryingMode Mode)
    : IRequest<GetAllEventsByUserQueryResult>;

public sealed record GetAllEventsByUserQueryResult(IEnumerable<EventQueryModel> Events);