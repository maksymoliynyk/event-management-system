using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using API.Extensions;

using Serilog;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.IO;
using System;
using FluentValidation.AspNetCore;
using FluentValidation;
using API.Validators;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Add services to the container.

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();
builder.Host.UseSerilog();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
    {
        options.SwaggerDoc("v1", new OpenApiInfo
        {
            Version = "v1",
            Title = "Event Management API",
            Description = "A simple manager for events"
        });
        string xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
        options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
    }
);

builder.Services.AddRouting(options => options.LowercaseUrls = true);

//* Extension Methods
builder.Services.RegisterMediatR();
builder.Services.RegisterRepositoryManager();
builder.Services.ConfigureDbContext(builder.Configuration);
builder.Services.ConfigureMapping();

builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<CreateEventValidator>();

WebApplication app = builder.Build();

app.UseSerilogRequestLogging();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    _ = app.UseSwagger();
    _ = app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

public partial class Program { }