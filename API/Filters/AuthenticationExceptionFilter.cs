using Domain.Exceptions;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace API.Filters
{
    public class AuthenticationExceptionFilter : IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            if (context.Exception is LoginException le)
            {
                context.Result = new UnauthorizedObjectResult(new { message = le.Message });
            }
            if (context.Exception is ObjectNotFoundException onfe)
            {
                context.Result = new NotFoundObjectResult(new { error = onfe.Error, message = onfe.Message });
            }
        }
    }
}