using System;
using Contracts.Models.Statuses;

namespace Contracts.Models
{
    public class Event
    {
        public string Id { get; init; }
        public string Title { get; init; }
        public string Description { get; init; }
        public DateTime Date { get; init; }
        public TimeSpan Duration { get; init; }
        public string Location { get; init; }
        public string OwnerEmail { get; init; }
        public EventStatus Status { get; init; }
    }
}