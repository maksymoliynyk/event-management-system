using System;

namespace Domain.Exceptions
{
    public enum RSPVSendExceptionError
    {
        UserIsOwner,
        UserAlreadyRSVPd
    }
    public class RSPVSendException : Exception
    {
        public RSPVSendExceptionError Error { get; init; }
        public RSPVSendException(RSPVSendExceptionError rSPVSendExceptionError, string message) : base(message)
        {
            Error = rSPVSendExceptionError;
        }
    }
}