using Microsoft.AspNetCore.Mvc;
using TaskManagement.Common.Middleware;
using TaskManagement.Common.ResultPattern;
using TaskManagement.Common.ResultPattern.Errors;
using TaskManagement.Users.Api.Mapping;
using TaskManagement.Users.Commands.GetUser;
using TaskManagement.Users.Contracts.Responses;
using TaskManagement.Users.Interfaces;
using TaskManagement.Users.Models;
using TaskManagement.Users.Services;

namespace TaskManagement.Users.Api.Endpoints;

public static class GetUserEndpoint
{
    public const string Name = "GetUser";

    public static IEndpointRouteBuilder MapGetUser(this IEndpointRouteBuilder app)
    {
        app.MapGet(ApiEndpoints.Users.Get, async (Guid id,
                [FromServices] IMediator mediator,
                [FromServices]ILogger<ILogger> logger,
                CancellationToken token) =>
            {
                var command = new GetUserCommand(id);
                var getUserResult = await mediator.SendAsync<GetUserCommand, Result<User>>(command, token);
                if (getUserResult.IsFailure && getUserResult.Errors.OfType<NotFoundError>().Any())
                {
                    return Results.NotFound(new { Message = "User not found" });
                }

                if (getUserResult.IsFailure)
                {
                    logger.LogError(string.Join(", ", getUserResult.Errors.Select(e => e.Message)));
                    return Results.Problem( "An error occurred while retrieving the user");
                }

                var response = getUserResult.Response.MapToResponse();
                return TypedResults.Ok(response);
            })
            .WithName(Name)
            .Produces<UserResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);
        return app;
    }
}