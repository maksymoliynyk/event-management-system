using System.Threading;
using System.Threading.Tasks;

using API.Filters;

using Contracts.RequestModels;

using Domain.Commands;
using Domain.Queries;

using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [TypeFilter(typeof(EventControllerExceptionFilter))]
    public class EventController : ControllerBase
    {
        private readonly IMediator _mediator;

        public EventController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Create an event
        /// </summary>
        /// <param name="request">Event information</param>
        /// <param name="cancellationToken">Cancellation Token</param>
        /// <returns>Id for created event</returns>
        /// <response code="201">Returns the newly created event</response>
        /// <response code="400">Bad request</response>
        /// <response code="500">Server error</response>

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateEvent([FromBody] CreateEventRequest request, CancellationToken cancellationToken = default)
        {
            CreateEventCommand command = new()
            {
                Title = request.Title,
                Description = request.Description,
                Date = request.Date,
                Duration = request.Duration,
                Location = request.Location,
                IsPublic = request.IsPublic,
                UserName = User.Identity.Name
            };
            CreateEventResult result = await _mediator.Send(command, cancellationToken);
            return Created($"{result.Id}", result);
        }

        /// <summary>
        /// Get Event By Id
        /// </summary>
        /// <param name="id">Id of searched event</param>
        /// <param name="cancellationToken">Cancellation Token</param>
        /// <returns>Information about event</returns>
        /// <response code="200">Returns the event</response>
        /// <response code="404">Event not found</response>
        /// <response code="500">Server error</response>
        [HttpGet("{id}")]
        [TypeFilter(typeof(EventExistFilter))]
        public async Task<IActionResult> GetEventById([FromRoute] string id, CancellationToken cancellationToken = default)
        {
            GetEventByIdQuery query = new()
            {
                Id = id,
                UserName = User.Identity.Name
            };
            GetEventByIdResult result = await _mediator.Send(query, cancellationToken);
            return Ok(result.SearchedEvent);
        }
        /// <summary>
        /// Delete Event By Id
        /// </summary>
        /// <param name="id">Id of searched event</param>
        /// <param name="cancellationToken">Cancellation Token</param>
        /// <returns></returns>
        /// <response code="204">Event deleted</response>
        /// <response code="404">Event not found</response>
        /// <response code="500">Server error</response>
        [HttpDelete("{id}")]
        [TypeFilter(typeof(EventExistFilter))]
        [Authorize]
        public async Task<IActionResult> DeleteEventById([FromRoute] string id, CancellationToken cancellationToken = default)
        {
            DeleteEventByIdCommand command = new()
            {
                Id = id,
                UserName = User.Identity.Name
            };
            _ = await _mediator.Send(command, cancellationToken);
            return NoContent();
        }
        /// <summary>
        /// Update Event By Id
        /// </summary>
        /// <param name="id">Id of searched event</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <response code="204">Event updated</response>
        /// <response code="400">Bad request</response>
        /// <response code="404">Event not found</response>
        /// <response code="500">Server error</response>
        [HttpPatch("{id}/cancel")]
        [TypeFilter(typeof(EventExistFilter))]
        [Authorize]
        public async Task<IActionResult> CancelEventById([FromRoute] string id, CancellationToken cancellationToken = default)
        {
            CancelEventByIdCommand command = new()
            {
                Id = id,
                UserName = User.Identity.Name
            };
            _ = await _mediator.Send(command, cancellationToken);
            return NoContent();
        }
        /// <summary>
        /// Send RSVP for event
        /// </summary>
        /// <param name="emailRequest">Email of invited user</param>
        /// <param name="id">Id of event</param>
        /// <param name="cancellationToken"></param>
        /// <returns>Id of RSVP</returns>
        /// <response code="201">RSVP created</response>
        /// <response code="400">Bad request</response>
        /// <response code="404">Event not found</response>
        /// <response code="500">Server error</response>
        [HttpPost("{id}/rsvp")]
        [TypeFilter(typeof(EventExistFilter))]
        [Authorize]
        public async Task<IActionResult> SendRSVP([FromBody] EmailRequest emailRequest, [FromRoute] string id, CancellationToken cancellationToken = default)
        {
            SendRSVPCommand request = new()
            {
                UserName = User.Identity.Name,
                UserEmail = emailRequest.Email,
                EventId = id
            };
            SendRSVPResult result = await _mediator.Send(request, cancellationToken);
            return Created($"{result.RSVPId}", result);
        }
        /// <summary>
        /// Get RSVPs for event
        /// </summary>
        /// <param name="id">Id of event</param>
        /// <param name="cancellationToken"></param>
        /// <returns>List of RSVPs for event</returns>
        /// <response code="200">Returns list of RSVPs</response>
        /// <response code="404">Event not found</response>
        /// <response code="500">Server error</response>
        [HttpGet("{id}/rsvps")]
        [TypeFilter(typeof(EventExistFilter))]
        [Authorize]
        public async Task<IActionResult> GetRSVPsForEvent([FromRoute] string id, CancellationToken cancellationToken = default)
        {
            GetEventsRSVPQuery query = new()
            {
                EventId = id,
                UserName = User.Identity.Name
            };
            GetEventsRSVPResult result = await _mediator.Send(query, cancellationToken);
            return Ok(result.RSVPs);
        }
    }
}