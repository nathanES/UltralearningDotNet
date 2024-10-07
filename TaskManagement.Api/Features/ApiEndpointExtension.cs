using TaskManagement.Api.Features.Tasks.Endpoints;
using TaskManagement.Api.Features.Users.Endpoints;

namespace TaskManagement.Api.Features;

public static class ApiEndpointExtension
{
    public static IEndpointRouteBuilder AddApiEndpoints(this IEndpointRouteBuilder app)
    {
        app.AddTasksEndpoints();
        app.AddUsersEndpoints();
        return app;
    }
}