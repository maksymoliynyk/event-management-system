using System.Threading;
using System.Threading.Tasks;

using MediatR;

using Microsoft.AspNetCore.Mvc;

using Domain.Queries;

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
        [HttpGet("{id}/events/history")]
        public async Task<IActionResult> GetHistoryOfCreatedEvents([FromRoute] string id, CancellationToken cancellationToken = default)
        {
            GetAllEventsCreatedByUserQuery request = new()
            {
                Id = id
            };
            GetAllEventsCreatedByUserResult result = await _mediator.Send(request, cancellationToken);
            return Ok(result.Events);
        }
        [HttpGet("{id}/events")]
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