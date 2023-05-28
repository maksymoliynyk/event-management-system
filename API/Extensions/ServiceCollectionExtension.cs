using System;
using System.IO;
using System.Reflection;
using System.Text;

using AutoMapper;

using Domain.Commands;
using Domain.DbContexts;
using Domain.Interfaces;
using Domain.MapperProfiles;
using Domain.Models.Database;
using Domain.Repositories;
using Domain.Services;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

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
                        map.AddProfile<RSVPProfile>();
                        map.AddProfile<UserProfile>();
                    }
                    );
                    return mc.CreateMapper();
                });
        }
        public static void ConfigureDbContext(this IServiceCollection services, IConfiguration configuration)
        {
            _ = services.AddDbContext<EventManagementContext>(
                options => options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"))
            );
        }

        public static void ConfigureSwagger(this IServiceCollection services)
        {
            _ = services.AddSwaggerGen(options =>
                    {
                        options.SwaggerDoc("v1", new OpenApiInfo
                        {
                            Version = "v1",
                            Title = "Event Management API",
                            Description = "A simple manager for events"
                        });
                        string xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                        options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
                        options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                        {
                            In = ParameterLocation.Header,
                            Description = "Please enter a valid token",
                            Name = "Authorization",
                            Type = SecuritySchemeType.Http,
                            BearerFormat = "JWT",
                            Scheme = "Bearer"
                        });
                        options.AddSecurityRequirement(new OpenApiSecurityRequirement
                                        {
                                            {
                                                new OpenApiSecurityScheme
                                                {
                                                    Reference = new OpenApiReference
                                                    {
                                                        Type=ReferenceType.SecurityScheme,
                                                        Id="Bearer"
                                                    }
                                                },
                                                Array.Empty<string>()
                                            }
                                        });
                    }
                );
        }
        public static void ConfigureIdentity(this IServiceCollection services)
        {
            _ = services.AddIdentityCore<UserDTO>(options =>
            {
                options.SignIn.RequireConfirmedAccount = false;
                options.User.RequireUniqueEmail = true;
                options.Password.RequireDigit = false;
                options.Password.RequiredLength = 6;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireLowercase = false;
            })
                .AddEntityFrameworkStores<EventManagementContext>()
                .AddDefaultTokenProviders();
        }

        public static void ConfigureAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            _ = services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(
                    options => options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ClockSkew = TimeSpan.Zero,
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = "apiWithAuthBackend",
                        ValidAudience = "apiWithAuthBackend",
                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(configuration["Authentication:SecretKey"])
                            )
                    });
        }

        public static void ConfigureAdditionalServices(this IServiceCollection services)
        {
            _ = services.AddScoped<AccessChecker>();
            _ = services.AddScoped<TokenService>();
        }

        public static void ConfigureCORS(this IServiceCollection services)
        {
            _ = services.AddCors(options => options.AddPolicy("AllowAllHeaders",
                                builder => _ = builder.AllowAnyOrigin()
                                        .AllowAnyHeader()
                                        .AllowAnyMethod()));
        }
    }
}