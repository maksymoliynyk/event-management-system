using System;

namespace Application.Models;

public class AttendeeQueryModel
{
    public string AttendeeEmail { get; init; }
    public Guid EventId { get; init; }
}