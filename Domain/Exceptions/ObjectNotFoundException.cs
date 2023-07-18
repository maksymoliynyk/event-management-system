using System;

namespace Domain.Exceptions
{
    public enum ObjectNotFoundErrors
    {
        User,
        Event,
        RSVP
    }
    public class ObjectNotFoundException : Exception
    {
        public ObjectNotFoundErrors Error { get; init; }
        public ObjectNotFoundException(string message, ObjectNotFoundErrors error) : base(message)
        {
            Error = error;
        }
    }
}