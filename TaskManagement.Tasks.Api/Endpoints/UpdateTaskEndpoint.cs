using Microsoft.AspNetCore.Mvc;
using TaskManagement.Tasks.Api.Mapping;
using TaskManagement.Tasks.Contracts.Requests;
using TaskManagement.Tasks.Contracts.Responses;
using TaskManagement.Tasks.Interfaces;
using TaskManagement.Tasks.Services;

namespace TaskManagement.Tasks.Api.Endpoints;

public static class UpdateTaskEndpoint
{
    private const string Name = "UpdateTask";

    public static IEndpointRouteBuilder MapUpdateTask(this IEndpointRouteBuilder app)
    {
        app.MapPut(ApiEndpoints.Tasks.Update, async (Guid id,
                [FromBody]UpdateTaskRequest request, 
                [FromServices] ITaskService taskService, CancellationToken token) =>
            {
                var task = request.MapToTask(id);
                var updatedTask = await taskService.UpdateAsync(task, token);
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