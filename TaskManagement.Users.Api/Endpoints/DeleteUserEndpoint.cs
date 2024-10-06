using Microsoft.AspNetCore.Mvc;
using TaskManagement.Common.Mediator;
using TaskManagement.Users.Commands.DeleteUser;
using TaskManagement.Users.Interfaces;
using TaskManagement.Users.Services;

namespace TaskManagement.Users.Api.Endpoints;

public static class DeleteUserEndpoint
{
    private const string Name = "DeleteUser";

    public static IEndpointRouteBuilder MapDeleteUser(this IEndpointRouteBuilder app)
    {
        app.MapDelete(ApiEndpoints.Users.Delete, async (Guid id,
                [FromServices] Mediator mediator, CancellationToken token) =>
            {
                var command = new DeleteUserCommand(id);
                var isDeleted = await mediator.SendAsync<DeleteUserCommand, bool>(command, token);
                if (!isDeleted)
                {
                    return Results.NotFound();
                }

                return Results.Ok();
            })
            .WithName(Name)
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);
        return app;
    }
}