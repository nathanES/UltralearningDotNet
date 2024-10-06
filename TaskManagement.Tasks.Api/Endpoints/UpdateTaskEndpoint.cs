using Microsoft.AspNetCore.Mvc;
using TaskManagement.Common.Mediator;
using TaskManagement.Tasks.Api.Mapping;
using TaskManagement.Tasks.Commands.UpdateTask;
using TaskManagement.Tasks.Contracts.Requests;
using TaskManagement.Tasks.Contracts.Responses;
using TaskManagement.Tasks.Interfaces;
using TaskManagement.Tasks.Services;
using Task = TaskManagement.Common.Models.Task;

namespace TaskManagement.Tasks.Api.Endpoints;

public static class UpdateTaskEndpoint
{
    private const string Name = "UpdateTask";

    public static IEndpointRouteBuilder MapUpdateTask(this IEndpointRouteBuilder app)
    {
        app.MapPut(ApiEndpoints.Tasks.Update, async (Guid id,
                [FromBody]UpdateTaskRequest request, 
                [FromServices] Mediator mediator, CancellationToken token) =>
            {
                var command = request.MapToCommand(id);
                var updatedTask = await mediator.SendAsync<UpdateTaskCommand, Task?>(command, token);
                if (updatedTask is null)
                {
                    return Results.NotFound();
                }
                var response = updatedTask?.MapToResponse();
                return TypedResults.Ok(response);
            })
            .WithName(Name)
            .Produces<TaskResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);
        return app;
    }
}