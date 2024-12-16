namespace Domain.Exceptions;

public class ObjectAlreadyExistException : Exception
{
    public EntitiesErrorType ErrorCode { get; }

    public ObjectAlreadyExistException(EntitiesErrorType errorCode)
        : base($"{errorCode} already exists.")
    {
        ErrorCode = errorCode;
    }
}