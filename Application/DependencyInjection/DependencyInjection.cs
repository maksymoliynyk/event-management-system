using Application.Commands.Events;

using Microsoft.Extensions.DependencyInjection;

namespace Application.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(c => c.RegisterServicesFromAssemblies(typeof(DependencyInjection).Assembly));

        return services;
    }
}