using System;
using System.IO;
using System.Reflection;
using System.Text;

using API.Validators;

using Application.Commands.AuthCommands;
using Application.Commands.EventCommands;
using Application.MapperProfiles;

using AutoMapper;

using Contracts.RequestModels;

using FluentValidation;

using Microsoft.AspNetCore.Authentication.JwtBearer;
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

        public static void ConfigureCORS(this IServiceCollection services)
        {
            _ = services.AddCors(options => options.AddPolicy("AllowAllHeaders",
                                builder => _ = builder.AllowAnyOrigin()
                                        .AllowAnyHeader()
                                        .AllowAnyMethod()));
        }
        public static void ConfigureValidation(this IServiceCollection services)
        {
            _ = services.AddScoped<IValidator<CreateEventRequest>, CreateEventValidator>();
            _ = services.AddScoped<IValidator<EmailRequest>, EmailRequestValidator>();
            _ = services.AddScoped<IValidator<LoginUserCommand>, LoginUserValidator>();
            _ = services.AddScoped<IValidator<RegisterUserCommand>, RegisterUserValidator>();
        }
    }
}