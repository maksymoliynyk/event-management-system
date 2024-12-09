using Domain.Exceptions;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace API.Filters;

public class DomainExceptionFilter : IExceptionFilter
{
    public void OnException(ExceptionContext context)
    {
        context.Result = context.Exception switch
        {
            ObjectAlreadyExistException oaee => new BadRequestObjectResult(new
            {
                errorCode = oaee.ErrorCode, message = oaee.Message
            }),
            ObjectNotFoundException onfe => new NotFoundObjectResult(new
            {
                errorCode = onfe.ErrorCode, message = onfe.Message
            }),
            LoginException le => new BadRequestObjectResult(new { message = le.Message }),
            _ => context.Result
        };
    }
}