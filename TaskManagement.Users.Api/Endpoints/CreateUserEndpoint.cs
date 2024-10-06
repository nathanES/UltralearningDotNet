using Microsoft.AspNetCore.Mvc;
using TaskManagement.Common.Mediator;
using TaskManagement.Users.Api.Mapping;
using TaskManagement.Users.Commands.CreateUser;
using TaskManagement.Users.Contracts.Requests;
using TaskManagement.Users.Contracts.Responses;

namespace TaskManagement.Users.Api.Endpoints;

public static class CreateUserEndpoint
{
    public const string Name = "CreateUser";

    public static IEndpointRouteBuilder MapCreateUser(this IEndpointRouteBuilder app)
    {
        app.MapPost(ApiEndpoints.Users.Create, async ([FromBody] CreateUserRequest request,
                [FromServices] Mediator mediator, CancellationToken token) =>
            {
                var command = request.MapToCommand();
                await mediator.SendAsync<CreateUserCommand, bool>(command, token);
                var response = new UserResponse
                {
                    Id = command.Id,
                    Email = command.Email,
                    Username = command.Username
                };
                return TypedResults.CreatedAtRoute(response, $"{CreateUserEndpoint.Name}V1", new { id = response.Id });
            })
            .WithName($"{Name}V1")
            .Produces<UserResponse>(StatusCodes.Status201Created)
            .WithApiVersionSet(ApiVersioning.VersionSet)
            .HasApiVersion(1.0);
        return app;
    }
}