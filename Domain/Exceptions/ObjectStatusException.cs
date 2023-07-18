using System;

namespace Domain.Exceptions
{
    public enum ObjectStatusError
    {
        StatusCannotBeChanged = 0,
        NewStatusCannotBeSameAsOldStatus
    }
    public enum ObjectType
    {
        Event,
        RSVP
    }
    public class ObjectStatusException : Exception
    {
        public ObjectStatusError Error { get; init; }
        public ObjectType Type { get; init; }
        public ObjectStatusException(string message, ObjectStatusError error, ObjectType type) : base(message)
        {
            Error = error;
            Type = type;
        }
    }
}