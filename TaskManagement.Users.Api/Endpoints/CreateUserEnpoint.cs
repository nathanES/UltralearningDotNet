using Microsoft.AspNetCore.Mvc;
using TaskManagement.Users.Api.Mapping;
using TaskManagement.Users.Contracts.Requests;
using TaskManagement.Users.Contracts.Responses;
using TaskManagement.Users.Services;

namespace TaskManagement.Users.Api.Endpoints;

public static class CreateUserEnpoint
{
    public const string Name = "CreateUser";

    public static IEndpointRouteBuilder MapCreateUser(this IEndpointRouteBuilder app)
    {
        app.MapPost(ApiEndpoints.Users.Create, async ([FromBody] CreateUserRequest request,
                [FromServices] IUserService userService, CancellationToken token) =>
            {
                var user = request.MapToUser();
                await userService.CreateAsync(user, token);
                var response = user.MapToResponse();
                return TypedResults.CreatedAtRoute(response, $"{CreateUserEnpoint.Name}V1", new { id = user.Id });
            })
            .WithName($"{Name}V1")
            .Produces<UserResponse>(StatusCodes.Status201Created)
            .WithApiVersionSet(ApiVersioning.VersionSet)
            .HasApiVersion(1.0);
        return app;
    }
}