using Microsoft.AspNetCore.Mvc;
using TaskManagement.Common.Middleware;
using TaskManagement.Common.ResultPattern;
using TaskManagement.Common.ResultPattern.Errors;
using TaskManagement.Tasks.Commands.DeleteTask;

namespace TaskManagement.Api.Features.Tasks.Endpoints;

public static class DeleteTaskEndpoint
{
    private const string Name = "DeleteTask";

    public static IEndpointRouteBuilder MapDeleteTask(this IEndpointRouteBuilder app)
    {
        app.MapDelete(ApiEndpoints.Tasks.Delete, async (Guid id,
                [FromServices]IMediator mediator,
                [FromServices]ILogger<ILogger> logger,
                CancellationToken token) =>
            {
                var command = new DeleteTaskCommand(id);
                var deleteTaskResult = await mediator.SendAsync<DeleteTaskCommand, Result<None>>(command, token);
                if (deleteTaskResult.IsFailure && deleteTaskResult.Errors.OfType<NotFoundError>().Any())
                {
                    return Results.NotFound(new { Message = "Task not found" });
                }

                if (deleteTaskResult.IsFailure)
                {
                    logger.LogError(string.Join(", ", deleteTaskResult.Errors.Select(e => e.Message)));
                    return Results.Problem( "An error occurred while deleting the task");
                }
                
                return Results.Ok();
            })
            .WithName(Name)
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);
        return app;
    }
}