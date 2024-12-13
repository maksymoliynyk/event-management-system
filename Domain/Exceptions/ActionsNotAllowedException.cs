namespace Domain.Exceptions;

public class ActionsNotAllowedException : Exception
{
    public EntitiesErrorType ErrorCode { get; }

    public ActionsNotAllowedException(EntitiesErrorType errorCode, string message)
        : base(message)
    {
        ErrorCode = errorCode;
    }
}