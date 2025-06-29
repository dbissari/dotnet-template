using Microsoft.AspNetCore.Routing;

namespace Infrastructure.ApiEndpoints;

public interface IEndpoint
{
    void MapEndpoint(IEndpointRouteBuilder app);
}
