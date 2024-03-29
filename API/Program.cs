using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using API.Extensions;

using Serilog;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Add services to the container.

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();
builder.Host.UseSerilog();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.ConfigureSwagger();

builder.Services.AddRouting(options => options.LowercaseUrls = true);

//* Extension Methods
builder.Services.ConfigureCORS();
builder.Services.RegisterMediatR();
builder.Services.RegisterRepositoryManager();
builder.Services.ConfigureDbContext(builder.Configuration);
builder.Services.ConfigureMapping();
builder.Services.ConfigureIdentity();
builder.Services.ConfigureAuthentication(builder.Configuration);
builder.Services.ConfigureAdditionalServices();
builder.Services.ConfigureValidation();

WebApplication app = builder.Build();

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

app.MapControllers();

app.Run();

public partial class Program { }