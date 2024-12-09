using Domain.Exceptions;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace API.Filters;

public class DomainExceptionFilter : IExceptionFilter
{
    public void OnException(ExceptionContext context)
    {
        if (context.Exception is ObjectAlreadyExistException oaee)
        {
            context.Result = new BadRequestObjectResult(new { errorCode = oaee.ErrorCode, message = oaee.Message });
        }

        if (context.Exception is ObjectNotFoundException onfe)
        {
            context.Result = new NotFoundObjectResult(new { errorCode = onfe.ErrorCode, message = onfe.Message });
        }
    }
}