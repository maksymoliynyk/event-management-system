using System.Threading;
using System.Threading.Tasks;

using MediatR;

using Microsoft.AspNetCore.Mvc;

using Domain.Queries;
using API.Filters;
using Contracts.RequestModels;

namespace API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IMediator _mediator;

        public UserController(IMediator mediator)
        {
            _mediator = mediator;
        }
        /// <summary>
        /// Get Id of user by email
        /// </summary>
        /// <param name="emailRequest"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>User's id</returns>
        /// <response code="200">Returns the id of user</response>
        /// <response code="400">Bad request</response>
        /// <response code="404">User not found</response>
        /// <response code="500">Server error</response>
        [HttpPost]
        //* temporary solution for user controllers
        public async Task<IActionResult> GetIdOfUserByEmail([FromBody] EmailRequest emailRequest, CancellationToken cancellationToken = default)
        {
            GetIdOfUserByEmailQuery request = new()
            {
                Email = emailRequest.Email
            };
            GetIdOfUserByEmailResult result = await _mediator.Send(request, cancellationToken);
            return Ok(result.Id);
        }
        /// <summary>
        /// Get history of created events
        /// </summary>
        /// <param name="id">Id of user</param>
        /// <param name="cancellationToken"></param>
        /// <returns>List of all created by user events</returns>
        /// <response code="200">Returns the list of events</response>
        /// <response code="404">User not found</response>
        /// <response code="500">Server error</response>
        [HttpGet("{id}/events/history")]
        [TypeFilter(typeof(UserExistFilter))]
        public async Task<IActionResult> GetHistoryOfCreatedEvents([FromRoute] string id, CancellationToken cancellationToken = default)
        {
            GetAllEventsCreatedByUserQuery request = new()
            {
                Id = id
            };
            GetAllEventsCreatedByUserResult result = await _mediator.Send(request, cancellationToken);
            return Ok(result.Events);
        }
        /// <summary>
        /// Get events that will take place and created by user
        /// </summary>
        /// <param name="id">User's id</param>
        /// <param name="cancellationToken"></param>
        /// <returns>List of events that will take place </returns>
        /// <response code="200">Returns the list of events</response>
        /// <response code="404">User not found</response>
        /// <response code="500">Server error</response>
        [HttpGet("{id}/events")]
        [TypeFilter(typeof(UserExistFilter))]
        public async Task<IActionResult> GetEventsThatWillTakePlace([FromRoute] string id, CancellationToken cancellationToken = default)
        {
            GetEventsCreatedByUserThatWillTakePlaceQuery request = new()
            {
                Id = id
            };
            GetEventsCreatedByUserThatWillTakePlaceResult result = await _mediator.Send(request, cancellationToken);
            return Ok(result.Events);
        }
    }
}