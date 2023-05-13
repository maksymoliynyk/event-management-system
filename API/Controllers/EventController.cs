using System.Threading;
using System.Threading.Tasks;

using Domain.Commands;
using Domain.Queries;

using MediatR;

using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class EventController : ControllerBase
    {
        private readonly IMediator _mediator;

        public EventController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> CreateEvent([FromBody] CreateEventCommand request, CancellationToken cancellationToken = default)
        {
            CreateEventResult result = await _mediator.Send(request, cancellationToken);
            return Created($"{result.Id}", result);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetEventById([FromRoute] string id, CancellationToken cancellationToken = default)
        {
            GetEventByIdQuery query = new()
            {
                Id = id
            };
            GetEventByIdResult result = await _mediator.Send(query, cancellationToken);
            return Ok(result.SearchedEvent);
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEventById([FromRoute] string id, CancellationToken cancellationToken = default)
        {
            DeleteEventByIdCommand command = new()
            {
                Id = id
            };
            await _mediator.Send(command, cancellationToken);
            return NoContent();
        }
        [HttpPatch]
        [Route("{id}/cancel")]
        public async Task<IActionResult> CancelEventById([FromRoute] string id, CancellationToken cancellationToken = default)
        {
            CancelEventByIdCommand command = new CancelEventByIdCommand
            {
                Id = id
            };
            await _mediator.Send(command, cancellationToken);
            return NoContent();
        }
    }
}