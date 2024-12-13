using System;
using System.Threading;
using System.Threading.Tasks;

using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using API.Models;

using Application.Commands.Events;
using Application.Commands.RSVPs;
using Application.Enums;
using Application.Queries.Attendees;
using Application.Queries.Events;
using Application.Queries.RSVPs;

using Microsoft.Extensions.Logging;

namespace API.Controllers
{
    [ApiController]
    [Authorize]
    [Route("[controller]")]
    public class EventsController : BaseController
    {
        public EventsController(ILogger<EventsController> logger, ISender sender) : base(logger, sender)
        {
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
        public async Task<IActionResult> CreateEvent([FromBody] CreateEventRequestModel request,
            CancellationToken cancellationToken)
        {
            var command = new CreateEventCommand(
                request.Title,
                request.Description,
                request.StartDate,
                request.Duration,
                request.Location,
                GetUserId());
            var result = await _sender.Send(command, cancellationToken);
            return Created($"{result.Id}", result.Id);
        }

        /// <summary>
        /// Get Event By Id
        /// </summary>
        /// <param name="eventId">Id of searched event</param>
        /// <param name="cancellationToken">Cancellation Token</param>
        /// <returns>Information about event</returns>
        /// <response code="200">Returns the event</response>
        /// <response code="404">Event not found</response>
        /// <response code="500">Server error</response>
        [HttpGet("{eventId:guid}")]
        public async Task<IActionResult> GetEventById([FromRoute] Guid eventId,
            CancellationToken cancellationToken)
        {
            var query = new GetEventByIdQuery(eventId, GetUserId());
            var result = await _sender.Send(query, cancellationToken);
            return Ok(result.Event);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAccessibleEvents([FromQuery] bool owner,
            CancellationToken cancellationToken)
        {
            var mode = owner ? EventQueryingMode.Owner : EventQueryingMode.Attendee;
            var byUserQuery = new GetAllEventsByUserQuery(GetUserId(), mode);
            var byUserQueryResult = await _sender.Send(byUserQuery, cancellationToken);
            return Ok(byUserQueryResult.Events);
        }

        /// <summary>
        /// Delete Event By Id
        /// </summary>
        /// <param name="eventId">Id of searched event</param>
        /// <param name="cancellationToken">Cancellation Token</param>
        /// <returns></returns>
        /// <response code="204">Event deleted</response>
        /// <response code="404">Event not found</response>
        /// <response code="500">Server error</response>
        [HttpDelete("{eventId:guid}")]
        public async Task<IActionResult> DeleteEventById([FromRoute] Guid eventId,
            CancellationToken cancellationToken)
        {
            var command = new DeleteEventByIdCommand(eventId, GetUserId());
            var result = await _sender.Send(command, cancellationToken);
            return NoContent();
        }

        /// <summary>
        /// Update Event By Id
        /// </summary>
        /// <param name="eventId">Id of searched event</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <response code="204">Event updated</response>
        /// <response code="400">Bad request</response>
        /// <response code="404">Event not found</response>
        /// <response code="500">Server error</response>
        [HttpPatch("{eventId:guid}/cancel")]
        public async Task<IActionResult> CancelEventById([FromRoute] Guid eventId,
            CancellationToken cancellationToken)
        {
            var command = new CancelEventByIdCommand(eventId, GetUserId());
            var result = await _sender.Send(command, cancellationToken);
            return Ok();
        }
        /// <summary>
        /// Invite user by email
        /// </summary>
        /// <param name="requestModel"></param>
        /// <param name="eventId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPost("{eventId:guid}/invites")]
        public async Task<IActionResult> SendRSVP([FromBody] SendRSVPRequestModel requestModel, [FromRoute] Guid eventId,
            CancellationToken cancellationToken)
        {
            var request = new SendRSVPCommand(GetUserId(), requestModel.Email, eventId);
            var result = await _sender.Send(request, cancellationToken);
            return Ok();
        }

        /// <summary>
        /// Get RSVPs for event
        /// </summary>
        /// <param name="eventId">Id of event</param>
        /// <param name="cancellationToken"></param>
        /// <returns>List of RSVPs for event</returns>
        /// <response code="200">Returns list of RSVPs</response>
        /// <response code="404">Event not found</response>
        /// <response code="500">Server error</response>
        [HttpGet("{eventId:guid}/invites")]
        public async Task<IActionResult> GetRSVPsForEvent([FromRoute] Guid eventId,
            CancellationToken cancellationToken)
        {
            var query = new GetEventsRSVPQuery(eventId, GetUserId());
            var queryResult = await _sender.Send(query, cancellationToken);
            return Ok(queryResult.RSVPs);
        }

        [HttpGet("{eventId:guid}/attendees")]
        public async Task<IActionResult> GetAttendeesForEvent([FromRoute] Guid eventId,
            CancellationToken cancellationToken)
        {
            var query = new GetAllAttendeesByEventQuery(eventId, GetUserId());
            var queryResult = await _sender.Send(query, cancellationToken);
            return Ok(queryResult.Attendees);
        }

        [HttpPatch("{eventId:guid}/invites")]
        public async Task<IActionResult> RespondToRsvp([FromRoute] Guid eventId, [FromBody] RespondToRSVPRequestModel model,
            CancellationToken cancellationToken)
        {
            var command = new UpdateRSVPStatusCommand(model.IsAccepted, eventId, GetUserId());
            var result = await _sender.Send(command, cancellationToken);
            return Ok();
        }
    }
}