using System;

namespace Contracts.RequestModels
{
    public class CreateEventRequest
    {
        public string Title { get; init; }
        public string Description { get; init; }
        public DateTime Date { get; init; }
        public long Duration { get; init; }
        public string Location { get; init; }
        public bool IsPublic { get; init; }
    }
}