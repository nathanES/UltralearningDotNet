using Microsoft.AspNetCore.Mvc;
using TaskManagement.Common.Mediator;
using TaskManagement.Common.Models;
using TaskManagement.Users.Api.Mapping;
using TaskManagement.Users.Commands.GetUser;
using TaskManagement.Users.Contracts.Responses;
using TaskManagement.Users.Interfaces;
using TaskManagement.Users.Services;

namespace TaskManagement.Users.Api.Endpoints;

public static class GetUserEndpoint
{
    public const string Name = "GetUser";

    public static IEndpointRouteBuilder MapGetUser(this IEndpointRouteBuilder app)
    {
        app.MapGet(ApiEndpoints.Users.Get, async (Guid id,
                [FromServices] Mediator mediator, CancellationToken token) =>
            {
                var command = new GetUserCommand(id);
                var user = await mediator.SendAsync<GetUserCommand, User?>(command, token);
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