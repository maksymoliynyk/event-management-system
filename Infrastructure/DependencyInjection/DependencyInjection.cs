using System.Text;

using Application.Interfaces.Repositories;

using Domain.Aggregates.Events;
using Domain.Entities.Users;
using Domain.Interfaces;

using Infrastructure.Identity;
using Infrastructure.Options;
using Infrastructure.Options.OptionsSetup;
using Infrastructure.Repositories;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Infrastructure.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection AddCustomOptions(this IServiceCollection services)
    {
        services.ConfigureOptions<JwtOptionSetup>();
        services.ConfigureOptions<ConnectionStringsOptionSetup>();

        return services;
    }

    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionStringsOption =
            services.BuildServiceProvider().GetRequiredService<IOptions<ConnectionStringsOption>>().Value;

        services.AddDbContext<EventManagementContext>(options =>
            options.UseSqlServer(connectionStringsOption.DefaultConnection));

        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IEventRepository, EventRepository>();
        services.AddScoped<IIdentityService, IdentityService>();
        services.AddScoped<ITokenProvider, TokenProvider>();

        services.AddScoped<IEventQueryRepository>(_ =>
        {
            var connectionString = connectionStringsOption.ReadOnlyConnection;
            var dbConnection = new SqlConnection(connectionString);
            return new EventQueryRepository(dbConnection);
        });

        return services;
    }

    public static IServiceCollection AddIdentity(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddIdentityCore<User>(options =>
            {
                options.SignIn.RequireConfirmedAccount = false;
                options.User.RequireUniqueEmail = true;
                options.Password.RequireDigit = false;
                options.Password.RequiredLength = 6;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireLowercase = false;
            })
            .AddEntityFrameworkStores<EventManagementContext>();

        services.AddAuthorization();
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                using var serviceProvider = services.BuildServiceProvider();
                var jwtOptions = serviceProvider.GetRequiredService<IOptions<JwtOption>>().Value;

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtOptions.Issuer,
                    ValidAudience = jwtOptions.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(jwtOptions.SecretKey))
                };
                options.UseSecurityTokenValidators = true;
                options.MapInboundClaims = false;
            });

        return services;
    }
}