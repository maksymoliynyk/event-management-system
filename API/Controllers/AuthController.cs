using System.Threading;
using System.Threading.Tasks;

using Application.Commands.Auth.Login;
using Application.Commands.Auth.Register;

using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : BaseController
    {
        public AuthController(ILogger<AuthController> logger, ISender sender) : base(logger, sender)
        {
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterUser([FromBody] RegisterUserCommand command,
            CancellationToken cancellationToken = default)
        {
            RegisterUserResult result = await _sender.Send(command, cancellationToken);
            return result.Succeeded ? Ok() : BadRequest(result.Errors);
        }

        [HttpPost("login")]
        public async Task<IActionResult> LoginUser([FromBody] LoginUserCommand command,
            CancellationToken cancellationToken = default)
        {
            LoginUserResult result = await _sender.Send(command, cancellationToken);
            return Ok(result.Token);
        }

        [HttpGet("is-correct")]
        [Authorize]
        public IActionResult IsCorrect()
        {
            return Ok();
        }
    }
}