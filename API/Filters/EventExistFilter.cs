using System.Threading.Tasks;

using Domain.Interfaces;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace API.Filters
{
    public class EventExistFilter : IAsyncActionFilter
    {
        private readonly IRepositoryManager _repositoryManager;

        public EventExistFilter(IRepositoryManager repositoryManager)
        {
            _repositoryManager = repositoryManager;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (context.RouteData.Values.TryGetValue("id", out object id))
            {
                Domain.Models.Database.EventDTO eventToFind = await _repositoryManager.Event.GetEventById(id.ToString(), default);
                if (eventToFind == null)
                {
                    context.Result = new NotFoundObjectResult("Event not found");
                    return;
                }
            }
            _ = await next();
        }
    }
}