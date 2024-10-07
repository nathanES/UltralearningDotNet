using Microsoft.AspNetCore.Mvc;
using TaskManagement.Common.Middleware;
using TaskManagement.Common.ResultPattern;
using TaskManagement.Users.Api.Mapping;
using TaskManagement.Users.Commands.CreateUser;
using TaskManagement.Users.Contracts.Requests;
using TaskManagement.Users.Contracts.Responses;
using TaskManagement.Users.Models;

namespace TaskManagement.Users.Api.Endpoints;

public static class CreateUserEndpoint
{
    public const string Name = "CreateUser";

    public static IEndpointRouteBuilder MapCreateUser(this IEndpointRouteBuilder app)
    {
        app.MapPost(ApiEndpoints.Users.Create, async ([FromBody] CreateUserRequest request,
                [FromServices] IMediator mediator, 
                [FromServices]ILogger<ILogger> logger,
                CancellationToken token) =>
            {
                var command = request.MapToCommand();
                var createUserResult = await mediator.SendAsync<CreateUserCommand, Result<User>>(command, token);
                if (createUserResult.IsFailure)
                {
                    logger.LogError(string.Join(", ", createUserResult.Errors.Select(e => e.Message)));
                    return Results.Problem( "An error occurred while creating the user");
                }
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