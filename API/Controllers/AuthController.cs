using System.Threading;
using System.Threading.Tasks;

using Domain.Commands.AuthCommands;

using MediatR;

using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [EnableCors("AllowAllHeaders")]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AuthController(IMediator mediator)
        {
            _mediator = mediator;
        }
        [HttpPost("register")]
        public async Task<IActionResult> RegisterUser([FromBody] RegisterUserCommand command, CancellationToken cancellationToken = default)
        {
            RegisterUserResult result = await _mediator.Send(command, cancellationToken);
            return result.Succeeded ? Ok() : BadRequest(result.Errors);
        }
        [HttpPost("login")]
        public async Task<IActionResult> LoginUser([FromBody] LoginUserCommand command, CancellationToken cancellationToken = default)
        {
            LoginUserResult result = await _mediator.Send(command, cancellationToken);
            return result.Error == null ? Ok(new { result.Token }) : BadRequest(new { result.Error });
        }

    }
}