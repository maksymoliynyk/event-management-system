using System.Threading.Tasks;
using System.Threading;

using Domain.Commands;

using MediatR;

using Microsoft.AspNetCore.Mvc;
using Domain.Queries;

namespace API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RSPVController : ControllerBase
    {
        private readonly IMediator _mediator;

        public RSPVController(IMediator mediator)
        {
            _mediator = mediator;
        }
        [HttpPost]
        public async Task<IActionResult> SendRSPV([FromBody] SendRSPVCommand request, CancellationToken cancellationToken = default)
        {
            SendRSPVResult result = await _mediator.Send(request, cancellationToken);
            return Created($"{result.RSPVId}", result);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetRSPVById([FromRoute] string id, CancellationToken cancellationToken = default)
        {
            GetRSPVByIdQuery request = new GetRSPVByIdQuery
            {
                RSPVId = id
            };
            GetRSPVByIdResult result = await _mediator.Send(request, cancellationToken);
            return Created($"{request.RSPVId}", result.RSPV);
        }
    }
}