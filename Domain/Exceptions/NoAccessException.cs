using System;

namespace Domain.Exceptions
{
    public class NoAccessException : Exception
    {
        public NoAccessException(string message) : base(message)
        {
        }
    }
}