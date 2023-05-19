using System.Threading.Tasks;

using Domain.Interfaces;
using Domain.Models.Database;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace API.Filters
{
    public class UserExistFilter : IAsyncActionFilter
    {
        private readonly IRepositoryManager _repositoryManager;

        public UserExistFilter(IRepositoryManager repositoryManager)
        {
            _repositoryManager = repositoryManager;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (context.RouteData.Values.TryGetValue("id", out object id))
            {
                UserDTO userToFind = await _repositoryManager.User.GetUserById(id.ToString(), default);
                if (userToFind == null)
                {
                    context.Result = new NotFoundObjectResult("User not found");
                    return;
                }
            }
            _ = await next();
        }
    }
}