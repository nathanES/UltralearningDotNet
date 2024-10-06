using Microsoft.AspNetCore.Mvc;
using TaskManagement.Common.Mediator;
using TaskManagement.Tasks.Commands.DeleteTask;
using TaskManagement.Tasks.Interfaces;
using TaskManagement.Tasks.Services;

namespace TaskManagement.Tasks.Api.Endpoints;

public static class DeleteTaskEndpoint
{
    private const string Name = "DeleteTask";

    public static IEndpointRouteBuilder MapDeleteTask(this IEndpointRouteBuilder app)
    {
        app.MapDelete(ApiEndpoints.Tasks.Delete, async (Guid id,
                [FromServices]Mediator mediator, CancellationToken token) =>
            {
                var command = new DeleteTaskCommand(id);
                var isDeleted = await mediator.SendAsync<DeleteTaskCommand, bool>(command, token);
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