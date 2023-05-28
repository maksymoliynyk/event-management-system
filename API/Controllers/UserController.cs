using System.Threading;
using System.Threading.Tasks;

using MediatR;

using Microsoft.AspNetCore.Mvc;

using Domain.Queries;
using Microsoft.AspNetCore.Authorization;

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
        /// Get history of created events
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns>List of all created by user events</returns>
        /// <response code="200">Returns the list of events</response>
        /// <response code="404">User not found</response>
        /// <response code="500">Server error</response>
        [HttpGet("events/history")]
        public async Task<IActionResult> GetHistoryOfCreatedEvents(CancellationToken cancellationToken = default)
        {
            GetAllEventsCreatedByUserQuery request = new()
            {
                UserName = User.Identity.Name
            };
            GetAllEventsCreatedByUserResult result = await _mediator.Send(request, cancellationToken);
            return Ok(result.Events);
        }
        /// <summary>
        /// Get events that will take place and created by user
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns>List of events that will take place </returns>
        /// <response code="200">Returns the list of events</response>
        /// <response code="404">User not found</response>
        /// <response code="500">Server error</response>
        [HttpGet("events")]
        public async Task<IActionResult> GetEventsThatWillTakePlace(CancellationToken cancellationToken = default)
        {
            GetEventsCreatedByUserThatWillTakePlaceQuery request = new()
            {
                UserName = User.Identity.Name,
            };
            GetEventsCreatedByUserThatWillTakePlaceResult result = await _mediator.Send(request, cancellationToken);
            return Ok(result.Events);
        }
    }
}