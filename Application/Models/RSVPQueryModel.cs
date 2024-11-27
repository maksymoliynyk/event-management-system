using System;

using Domain.Enums;

namespace Application.Queries.Models;

public class RSVPQueryModel
{
    public string InviteeEmail { get; init; }
    public Guid EventId { get; init; }
    public RSVPStatus RSVPStatus { get; init; }
}