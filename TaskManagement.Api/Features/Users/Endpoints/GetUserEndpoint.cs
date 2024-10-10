using Microsoft.AspNetCore.Mvc;
using TaskManagement.Api.Auth;
using TaskManagement.Api.Cache;
using TaskManagement.Api.Features.Users.Mapping;
using TaskManagement.Common.Middleware;
using TaskManagement.Common.Models;
using TaskManagement.Common.ResultPattern;
using TaskManagement.Common.ResultPattern.Errors;
using TaskManagement.Users.Commands.GetUser;
using TaskManagement.Users.Contracts.Responses;

namespace TaskManagement.Api.Features.Users.Endpoints;

public static class GetUserEndpoint
{
    public const string Name = "GetUser";

    public static IEndpointRouteBuilder MapGetUser(this IEndpointRouteBuilder app)
    {
        app.MapGet(ApiEndpoints.Users.Get, async (Guid id,
                [FromServices] IMediator mediator,
                [FromServices] ILogger<ILogger> logger,
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
                    return Results.Problem("An error occurred while retrieving the user");
                }

                var response = getUserResult.Response.MapToResponse();
                return TypedResults.Ok(response);
            })
            .CacheOutput(PolicyConstants.GetUserCache.name)
            .WithName(Name)
            .Produces<UserResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);
        return app;
    }
}