using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.JsonWebTokens;

namespace API.Middlewares;

public class UserContextMiddleware
{
    private readonly RequestDelegate _next;

    public UserContextMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var userIdClaim = context.User.FindFirst(JwtRegisteredClaimNames.Sub);

        if (!string.IsNullOrEmpty(userIdClaim?.Value))
        {
            context.Items["UserId"] = userIdClaim?.Value;
        }

        await _next(context);
    }
}
