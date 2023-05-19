using System;

namespace Domain.Exceptions
{
    public enum EventStatusError
    {
        StatusCannotBeChanged,
        NewStatusCannotBeSameAsOldStatus
    }
    public class EventStatusException : Exception
    {
        public EventStatusError Error { get; init; }
        public EventStatusException(EventStatusError eventStatusError, string message) : base(message)
        {
            Error = eventStatusError;
        }
    }
}