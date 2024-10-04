using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using TaskManagement.Tasks.Api.Mapping;
using TaskManagement.Tasks.Contracts.Requests;
using TaskManagement.Tasks.Contracts.Responses;
using TaskManagement.Tasks.Services;

namespace TaskManagement.Tasks.Api.Endpoints;

public static class CreateTaskEnpoint
{
    public const string Name = "CreateTask";

    public static IEndpointRouteBuilder MapCreateTask(this IEndpointRouteBuilder app)
    {
        app.MapPost(ApiEndpoints.Tasks.Create, async ([FromBody] CreateTaskRequest request,
                [FromServices] ITaskService taskService, CancellationToken token) =>
            {
                var task = request.MapToTask();
                await taskService.CreateAsync(task, token);
                var response = task.MapToResponse();
                return TypedResults.CreatedAtRoute(response, $"{CreateTaskEnpoint.Name}V1", new { id = task.Id });
            })
            .WithName($"{Name}V1")
            .Produces<TaskResponse>(StatusCodes.Status201Created)
            .WithApiVersionSet(ApiVersioning.VersionSet)
            .HasApiVersion(1.0);
        
        app.MapPost(ApiEndpoints.Tasks.Create, async ([FromBody] CreateTaskRequest request,
                [FromServices] ITaskService taskService, CancellationToken token) =>
            {
                var task = request.MapToTask();
                await taskService.CreateAsync(task, token);
                var response = task.MapToResponse();
                return TypedResults.CreatedAtRoute(response, $"{CreateTaskEnpoint.Name}V2", new { id = task.Id });
            })
            .WithName($"{Name}V2")
            .Produces<TaskResponse>(StatusCodes.Status201Created)
            .WithApiVersionSet(ApiVersioning.VersionSet)
            .HasApiVersion(2.0);
        return app;
    }
}