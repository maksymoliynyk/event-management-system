using System.Threading;
using System.Threading.Tasks;

using API.Extensions;

using Application.Commands.RSVPCommands;
using Application.Queries.RSVPQueries;

using Contracts.RequestModels;

using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class RSVPController : ControllerBase
    {
        private readonly IMediator _mediator;
        public RSVPController(IMediator mediator)
        {
            _mediator = mediator;
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetRSVPById([FromRoute] string id, CancellationToken cancellationToken = default)
        {
            GetRSVPByIdQuery query = new()
            {
                RSVPId = id,
                UserId = User.GetUserIdFromJWT()
            };
            GetRSVPByIdResult result = await _mediator.Send(query, cancellationToken);
            return Ok(result.RSVP);
        }
        [HttpPatch("{id}")]
        public async Task<IActionResult> ChangeRSVPStatus([FromRoute] string id, [FromBody] RSVPAcceptRequest request, CancellationToken cancellationToken = default)
        {
            UpdateRSVPStatusCommand command = new()
            {
                RSVPAccepted = request.AcceptInvite,
                RSVPId = id,
                UserId = User.GetUserIdFromJWT()
            };
            _ = await _mediator.Send(command, cancellationToken);
            return NoContent();
        }
    }
}