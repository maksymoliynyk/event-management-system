using System.Threading;
using System.Threading.Tasks;

using Application.Queries.RSVPs;

using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace API.Controllers
{
    [ApiController]
    [Route("invites")]
    [Authorize]
    public class RSVPController : BaseController
    {
        public RSVPController(ILogger<RSVPController> logger, ISender sender) : base(logger, sender)
        {
        }

        [HttpGet]
        public async Task<IActionResult> GetRSVPsByUser(CancellationToken cancellationToken)
        {
            var query = new GetUsersRSVPQuery(GetUserId());
            var result = await _sender.Send(query, cancellationToken);
            return Ok(result.RSVPs);
        }
    }
}