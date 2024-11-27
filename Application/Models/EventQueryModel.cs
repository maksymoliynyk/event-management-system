using System;

using Domain.Enums;

namespace Application.Queries.Models;

public class EventQueryModel
{
    public Guid Id { get; init; }
    public string Title { get; init; }
    public string Description { get; init; }
    public DateTime StartDate { get; init; }
    public TimeSpan Duration { get; init; }
    public string Location { get; init; }
    public EventStatus Status { get; init; }
    public DateTime CreateDate { get; init; }
}