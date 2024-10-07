using Microsoft.AspNetCore.Mvc;
using TaskManagement.Common.Middleware;
using TaskManagement.Common.ResultPattern;
using TaskManagement.Common.ResultPattern.Errors;
using TaskManagement.Users.Commands.DeleteUser;

namespace TaskManagement.Api.Features.Users.Endpoints;

public static class DeleteUserEndpoint
{
    private const string Name = "DeleteUser";

    public static IEndpointRouteBuilder MapDeleteUser(this IEndpointRouteBuilder app)
    {
        app.MapDelete(ApiEndpoints.Users.Delete, async (Guid id,
                [FromServices] IMediator mediator,
                [FromServices]ILogger<ILogger> logger,
                CancellationToken token) =>
            {
                var command = new DeleteUserCommand(id);
                var deleteUserResult = await mediator.SendAsync<DeleteUserCommand, Result<None>>(command, token);
                if (deleteUserResult.IsFailure && deleteUserResult.Errors.OfType<NotFoundError>().Any())
                {
                    return Results.NotFound(new { Message = "User not found" });
                }

                if (deleteUserResult.IsFailure)
                {
                    logger.LogError(string.Join(", ", deleteUserResult.Errors.Select(e => e.Message)));
                    return Results.Problem( "An error occurred while deleting the user");
                }

                return Results.Ok();
            })
            .WithName(Name)
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);
        return app;
    }
}