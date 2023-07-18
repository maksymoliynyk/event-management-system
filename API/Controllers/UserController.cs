using System.Threading;
using System.Threading.Tasks;

using MediatR;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Domain.Queries.UserQueries;
using API.Extensions;
using Domain.Queries.RSVPQueries;
using System.Linq;
using Contracts.Models;

namespace API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly IMediator _mediator;

        public UserController(IMediator mediator)
        {
            _mediator = mediator;
        }
        /// <summary>
        /// Get info about user
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns>User's id</returns>
        /// <response code="200">Returns the id of user</response>
        /// <response code="400">Bad request</response>
        /// <response code="404">User not found</response>
        /// <response code="500">Server error</response>
        [HttpGet]
        public async Task<IActionResult> GetInfoAboutUser(CancellationToken cancellationToken = default)
        {
            GetInfoAboutUserQuery request = new()
            {
                UserName = User.Identity.Name
            };
            GetInfoAboutUserResult result = await _mediator.Send(request, cancellationToken);
            return Ok(result);
        }
        /// <summary>
        /// Get events that will take place and created by user
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns>List of events that will take place </returns>
        /// <response code="200">Returns the list of events</response>
        /// <response code="404">User not found</response>
        /// <response code="500">Server error</response>
        [HttpGet("events/created")]
        public async Task<IActionResult> GetEventsThatCreatedByUser(CancellationToken cancellationToken = default)
        {
            GetAllEventsCreatedByUserQuery request = new()
            {
                UserId = User.GetUserIdFromJWT(),
            };
            GetAllEventsCreatedByUserResult result = await _mediator.Send(request, cancellationToken);
            return result.Events.Equals(Enumerable.Empty<Event>()) ? NoContent() : Ok(result.Events);
        }

        [HttpGet("events/participated")]
        [Authorize]
        public async Task<IActionResult> GetAllEventsWhereUserAreParticipate(CancellationToken cancellationToken = default)
        {
            GetAllEventsWhereUserAreParticipateQuery query = new()
            {
                UserId = User.GetUserIdFromJWT()
            };
            GetAllEventsWhereUserAreParticipateResult result = await _mediator.Send(query, cancellationToken);
            return result.Events.Equals(Enumerable.Empty<Event>()) ? NoContent() : Ok(result.Events);
        }
        [HttpGet("rsvps")]
        [Authorize]
        public async Task<IActionResult> GetAllUsersRSVPs(CancellationToken cancellationToken = default)
        {
            GetUsersRSVPQuery query = new()
            {
                UserId = User.GetUserIdFromJWT()
            };
            GetUsersRSVPResult result = await _mediator.Send(query, cancellationToken);
            return result.RSVPs.Equals(Enumerable.Empty<RSVP>()) ? NoContent() : Ok(result.RSVPs);
        }
    }
}