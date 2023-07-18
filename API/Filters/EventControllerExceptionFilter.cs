using Domain.Exceptions;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace API.Filters
{
    public class EventControllerExceptionFilter : IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            if (context.Exception is ObjectStatusException eventStatusException)
            {
                context.Result = new BadRequestObjectResult(new { error = eventStatusException.Error, message = eventStatusException.Message });
            }
            if (context.Exception is RSPVSendException rSPVSendException)
            {
                context.Result = new BadRequestObjectResult(new { error = rSPVSendException.Error, message = rSPVSendException.Message });
            }
            if (context.Exception is NoAccessException)
            {
                context.Result = new ForbidResult();
            }
            if (context.Exception is ObjectNotFoundException objectNotFoundException)
            {
                context.Result = new NotFoundObjectResult(new { error = objectNotFoundException.Error, message = context.Exception.Message });
            }
        }
    }
}