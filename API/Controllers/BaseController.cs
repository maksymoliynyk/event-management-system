using System;

using MediatR;

using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace API.Controllers;

[EnableCors("AllowAllHeaders")]
public class BaseController : ControllerBase
{
    protected readonly ILogger<BaseController> _logger;
    protected readonly ISender _sender;

    public BaseController(ILogger<BaseController> logger, ISender sender)
    {
        _logger = logger;
        _sender = sender;
    }

    protected Guid GetUserId()
    {
        var userIdClaim = HttpContext.Items["UserId"].ToString();

        if (string.IsNullOrEmpty(userIdClaim))
        {
            throw new Exception("User ID not found in the token.");
        }

        return Guid.Parse(userIdClaim);
    }
}