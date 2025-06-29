using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Infrastructure.ApiEndpoints;

public static class EndpointsRegistration
{
    public static IServiceCollection AddEndpoints(
        this IServiceCollection services,
        params Assembly[] assemblies
    )
    {
        static bool IsTypeEndpoint(TypeInfo type) =>
            type is { IsAbstract: false, IsInterface: false }
            && type.IsAssignableTo(typeof(IEndpoint));

        static ServiceDescriptor ServiceDescriptorFactory(TypeInfo type) =>
            ServiceDescriptor.Transient(typeof(IEndpoint), type);

        var serviceDescriptors = assemblies
            .SelectMany(assembly => assembly.DefinedTypes.Where(IsTypeEndpoint))
            .Select(ServiceDescriptorFactory)
            .ToArray();

        services.TryAddEnumerable(serviceDescriptors);

        return services;
    }

    public static IApplicationBuilder MapEndpoints(
        [NotNull] this WebApplication app,
        RouteGroupBuilder? routeGroupBuilder = null
    )
    {
        var endpoints = app.Services.GetServices<IEndpoint>();

        IEndpointRouteBuilder builder = (IEndpointRouteBuilder?)routeGroupBuilder ?? app;

        foreach (IEndpoint endpoint in endpoints)
        {
            endpoint.MapEndpoint(builder);
        }

        return app;
    }
}
