using Microsoft.AspNetCore.Mvc;
using TaskManagement.Tasks.Interfaces;
using TaskManagement.Tasks.Services;

namespace TaskManagement.Tasks.Api.Endpoints;

public static class DeleteTaskEndpoint
{
    private const string Name = "DeleteTask";

    public static IEndpointRouteBuilder MapDeleteTask(this IEndpointRouteBuilder app)
    {
        app.MapDelete(ApiEndpoints.Tasks.Delete, async (Guid id,
                [FromServices] ITaskService taskService, CancellationToken token) =>
            {
                var isDeleted = await taskService.DeleteByIdAsync(id, token);
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