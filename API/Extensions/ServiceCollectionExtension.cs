using System;
using System.IO;
using System.Reflection;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi;

namespace API.Extensions
{
    public static class ServiceCollectionExtension
    {
        extension(IServiceCollection services)
        {
            public void ConfigureSwagger()
            {
                _ = services.AddSwaggerGen(options =>
                    {
                        options.SwaggerDoc("v1",
                            new OpenApiInfo
                            {
                                Version = "v1",
                                Title = "Event Management API",
                                Description = "A simple manager for events"
                            });
                        string xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                        options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
                        options.AddSecurityDefinition("Bearer",
                            new OpenApiSecurityScheme
                            {
                                In = ParameterLocation.Header,
                                Description = "Please enter a valid token",
                                Name = "Authorization",
                                Type = SecuritySchemeType.Http,
                                BearerFormat = "JWT",
                                Scheme = "Bearer"
                            });
                        options.AddSecurityRequirement(doc => new OpenApiSecurityRequirement
                        {
                            {
                                (OpenApiSecuritySchemeReference)doc.Components.SecuritySchemes["Bearer"], []
                            }
                        });
                    }
                );
            }

            public void ConfigureCors()
            {
                _ = services.AddCors(options => options.AddPolicy("AllowAllHeaders",
                    builder => _ = builder.AllowAnyOrigin()
                        .AllowAnyHeader()
                        .AllowAnyMethod()));
            }
        }
    }
}