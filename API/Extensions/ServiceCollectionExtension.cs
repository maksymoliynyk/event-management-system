using AutoMapper;

using Domain.Commands;
using Domain.DbContexts;
using Domain.Interfaces;
using Domain.MapperProfiles;
using Domain.Repositories;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace API.Extensions
{
    public static class ServiceCollectionExtension
    {
        public static void RegisterMediatR(this IServiceCollection services)
        {
            _ = services.AddMediatR(c => c.RegisterServicesFromAssemblies(typeof(CreateEventCommand).Assembly));
        }
        public static void RegisterRepositoryManager(this IServiceCollection services)
        {
            _ = services.AddScoped<IRepositoryManager, RepositoryManager>();
        }
        public static void ConfigureMapping(this IServiceCollection services)
        {
            _ = services.AddSingleton(_ =>
                {
                    MapperConfiguration mc = new(map =>
                    {
                        map.AddProfile<EventProfile>();
                        map.AddProfile<RSPVProfile>();
                    }
                    );
                    return mc.CreateMapper();
                }
);
        }
        public static void ConfigureDbContext(this IServiceCollection services, IConfiguration configuration)
        {
            _ = services.AddDbContext<EventManagementContext>(
                options => options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"))
            );
        }
    }
}