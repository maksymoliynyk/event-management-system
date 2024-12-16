using System;

using Application.Models;

namespace Application.Queries.Events;

public sealed record GetEventByIdQuery(Guid Id, Guid UserId) : IRequest<GetEventByIdQueryResult>;

public sealed record GetEventByIdQueryResult(EventQueryModel Event);