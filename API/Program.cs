using AutoMapper;

using Domain.Commands;
using Domain.DbContexts;
using Domain.Repositories;

using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using Domain.MapperProfiles;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ToDo: Implement configuration
builder.Services.AddRouting(options => options.LowercaseUrls = true);

builder.Services.AddMediatR(c => c.RegisterServicesFromAssemblies(typeof(CreateEventCommand).Assembly));
builder.Services.AddScoped<IRepository, Repository>();
builder.Services.AddDbContext<EventManagementContext>(
    options => options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
);
builder.Services.AddSingleton(_ =>
    {
        MapperConfiguration mc = new(cfg => cfg.AddProfile<EventProfile>());
        return mc.CreateMapper();
    }
);

WebApplication app = builder.Build();

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
