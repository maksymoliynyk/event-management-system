using System;

namespace Domain.Exceptions
{
    public enum LoginExceptionError
    {
        UserNotFound,
        PasswordIncorrect
    }
    public class LoginException : Exception
    {
        public LoginExceptionError Error { get; init; }
        public LoginException(LoginExceptionError loginExceptionError, string message) : base(message)
        {
            Error = loginExceptionError;
        }
    }
}