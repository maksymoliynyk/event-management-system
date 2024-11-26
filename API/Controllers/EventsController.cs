using System.Threading;
using System.Threading.Tasks;

using API.Filters;
using API.Extensions;

using Contracts.RequestModels;

using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FluentValidation;
using FluentValidation.Results;
using System.Linq;
using Contracts.Models;
using Application.Commands.EventCommands;
using Application.Queries.EventQueries;
using Application.Commands.RSVPCommands;
using Application.Queries.RSVPQueries;

namespace API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [TypeFilter(typeof(EventControllerExceptionFilter))]
    public class EventsController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IValidator<CreateEventRequest> _createEventValidator;
        private readonly IValidator<EmailRequest> _emailRequestValidator;

        public EventsController(IMediator mediator
        , IValidator<CreateEventRequest> createEventValidator, IValidator<EmailRequest> emailRequestValidator
        )
        {
            _mediator = mediator;
            _createEventValidator = createEventValidator;
            _emailRequestValidator = emailRequestValidator;
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
            ValidationResult validationResult = await _createEventValidator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                validationResult.AddToModelState(ModelState);
                return BadRequest(ModelState);
            }
            CreateEventCommand command = new()
            {
                Title = request.Title,
                Description = request.Description,
                Date = request.Date,
                Duration = request.Duration,
                Location = request.Location,
                IsPublic = request.IsPublic,
                UserId = User.GetUserIdFromJWT()
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
        public async Task<IActionResult> GetEventById([FromRoute] string id, CancellationToken cancellationToken = default)
        {
            GetEventByIdQuery query = new()
            {
                Id = id,
                UserId = User.GetUserIdFromJWT()
            };
            GetEventByIdResult result = await _mediator.Send(query, cancellationToken);
            return Ok(result.SearchedEvent);
        }

        [HttpGet("events")]
        public async Task<IActionResult> GetAllAccessibleEvents(CancellationToken cancellationToken = default)
        {
            GetAllAccessibleEventsQuery query = new()
            {
                UserId = User.GetUserIdFromJWT()
            };
            GetAllAccessibleEventsResult result = await _mediator.Send(query, cancellationToken);
            return result.Events.Equals(Enumerable.Empty<Event>()) ? NoContent() : Ok(result.Events);
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
        [Authorize]
        public async Task<IActionResult> DeleteEventById([FromRoute] string id, CancellationToken cancellationToken = default)
        {
            DeleteEventByIdCommand command = new()
            {
                Id = id,
                UserId = User.GetUserIdFromJWT()
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
        [Authorize]
        public async Task<IActionResult> CancelEventById([FromRoute] string id, CancellationToken cancellationToken = default)
        {
            CancelEventByIdCommand command = new()
            {
                Id = id,
                UserId = User.GetUserIdFromJWT()
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
        [HttpPost("{id}/invite")]
        [Authorize]
        public async Task<IActionResult> SendRSVP([FromBody] EmailRequest emailRequest, [FromRoute] string id, CancellationToken cancellationToken = default)
        {
            ValidationResult validationResult = await _emailRequestValidator.ValidateAsync(emailRequest, cancellationToken);
            if (!validationResult.IsValid)
            {
                validationResult.AddToModelState(ModelState);
                return BadRequest(ModelState);
            }
            SendRSVPCommand request = new()
            {
                UserOwnerId = User.GetUserIdFromJWT(),
                UserInviteEmail = emailRequest.Email,
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
        [HttpGet("{id}/invited")]
        [Authorize]
        public async Task<IActionResult> GetRSVPsForEvent([FromRoute] string id, CancellationToken cancellationToken = default)
        {
            GetEventsRSVPQuery query = new()
            {
                EventId = id,
                UserId = User.GetUserIdFromJWT()
            };
            GetEventsRSVPResult result = await _mediator.Send(query, cancellationToken);
            return result.RSVPs.Equals(Enumerable.Empty<RSVP>()) ? NoContent() : Ok(result.RSVPs);
        }
    }
}