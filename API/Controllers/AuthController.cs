using System.Threading;
using System.Threading.Tasks;

using API.Extensions;
using API.Filters;

using Application.Commands.Auth;

using FluentValidation;
using FluentValidation.Results;

using MediatR;

using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [TypeFilter(typeof(AuthenticationExceptionFilter))]
    public class AuthController : BaseController
    {
        private readonly IValidator<LoginUserCommand> _loginValidator;
        private readonly IValidator<RegisterUserCommand> _registerValidator;

        public AuthController(ILogger<AuthController> logger, ISender sender,
            IValidator<LoginUserCommand> loginValidator,
            IValidator<RegisterUserCommand> registerValidator) : base(logger, sender)
        {
            _loginValidator = loginValidator;
            _registerValidator = registerValidator;
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterUser([FromBody] RegisterUserCommand command,
            CancellationToken cancellationToken = default)
        {
            ValidationResult validationResult = await _registerValidator.ValidateAsync(command, cancellationToken);
            if (!validationResult.IsValid)
            {
                validationResult.AddToModelState(ModelState);
                return BadRequest(ModelState);
            }

            RegisterUserResult result = await _sender.Send(command, cancellationToken);
            return result.Succeeded ? Ok() : BadRequest(result.Errors);
        }

        [HttpPost("login")]
        public async Task<IActionResult> LoginUser([FromBody] LoginUserCommand command,
            CancellationToken cancellationToken = default)
        {
            ValidationResult validationResult = await _loginValidator.ValidateAsync(command, cancellationToken);
            if (!validationResult.IsValid)
            {
                validationResult.AddToModelState(ModelState);
                return BadRequest(ModelState);
            }

            LoginUserResult result = await _sender.Send(command, cancellationToken);
            return Ok(result.Token);
        }
    }
}