using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using API.Extensions;
using API.Middleware;

using Application.DependencyInjection;

using Infrastructure.DependencyInjection;

using Serilog;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, loggerConfig) =>
{
    loggerConfig.ReadFrom.Configuration(context.Configuration);
});

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.ConfigureSwagger();

builder.Services.AddRouting(options => options.LowercaseUrls = true);

//* Extension Methods
builder.Services.ConfigureCORS();
builder.Services.ConfigureValidation();

builder.Services.AddCustomOptions()
    .AddIdentity(builder.Configuration)
    .AddInfrastructure(builder.Configuration)
    .AddApplication();

var app = builder.Build();

app.UseSerilogRequestLogging();

app.UseCors("AllowAllHeaders");
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    _ = app.UseSwagger();
    _ = app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.UseMiddleware<UserContextMiddleware>();

app.MapControllers();

app.Run();

public partial class Program
{
}