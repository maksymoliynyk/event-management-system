using System.Reflection;

using API;

using Application.Interfaces.Repositories;

using Domain.Aggregates.Events;

using Infrastructure;

namespace ArchitectureTests;

public static class Assemblies
{
    public static readonly Assembly DomainAssembly = typeof(Event).Assembly;
    public static readonly Assembly InfrastructureAssembly = typeof(UnitOfWork).Assembly;
    public static readonly Assembly ApplicationAssembly = typeof(IEventQueryRepository).Assembly;
    public static readonly Assembly ApiAssembly = typeof(Program).Assembly;
}