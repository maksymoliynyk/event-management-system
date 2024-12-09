namespace Domain.Exceptions;

public class ObjectNotFoundException : Exception
{
    public EntitiesErrorType ErrorCode { get; }

    public ObjectNotFoundException(EntitiesErrorType errorCode)
        : base($"{errorCode} not found.")
    {
        ErrorCode = errorCode;
    }
}