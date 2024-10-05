using Microsoft.AspNetCore.Mvc;
using TaskManagement.Users.Api.Mapping;
using TaskManagement.Users.Contracts.Responses;
using TaskManagement.Users.Services;

namespace TaskManagement.Users.Api.Endpoints;

public static class GetUserEndpoint
{
    public const string Name = "GetUser";

    public static IEndpointRouteBuilder MapGetUser(this IEndpointRouteBuilder app)
    {
        app.MapGet(ApiEndpoints.Users.Get, async (Guid id,
                [FromServices] IUserService userService, CancellationToken token) =>
            {
                var user = await userService.GetByIdAsync(id, token);
                if (user is null)
                {
                    return Results.NotFound();
                }

                var response = user?.MapToResponse();
                return TypedResults.Ok(response);
            })
            .WithName(Name)
            .Produces<UserResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);
        return app;
    }
}