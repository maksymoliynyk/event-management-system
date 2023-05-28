using Domain.Exceptions;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

using System;

namespace API.Filters
{
    public class EventControllerExceptionFilter : IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            if (context.Exception is EventStatusException eventStatusException)
            {
                context.Result = eventStatusException.Error switch
                {
                    EventStatusError.StatusCannotBeChanged => new BadRequestObjectResult(new { message = eventStatusException.Message }),
                    EventStatusError.NewStatusCannotBeSameAsOldStatus => new BadRequestObjectResult(new { message = eventStatusException.Message }),
                    _ => throw new Exception(context.Exception.Message, context.Exception.InnerException)
                };
            }
            if (context.Exception is RSPVSendException rSPVSendException)
            {
                context.Result = rSPVSendException.Error switch
                {
                    RSPVSendExceptionError.UserIsOwner => new BadRequestObjectResult(new { message = rSPVSendException.Message }),
                    RSPVSendExceptionError.UserAlreadyRSVPd => new BadRequestObjectResult(new { message = rSPVSendException.Message }),
                    _ => throw new Exception(context.Exception.Message, context.Exception.InnerException)
                };
            }

            if (context.Exception is NoAccessException)
            {
                context.Result = new ForbidResult();
            }
        }
    }
}