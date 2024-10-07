using Microsoft.AspNetCore.Mvc;
using TaskManagement.Api.Features.Tasks.Mapping;
using TaskManagement.Common.Middleware;
using TaskManagement.Common.ResultPattern;
using TaskManagement.Tasks.Commands.CreateTask;
using TaskManagement.Tasks.Contracts.Requests;
using TaskManagement.Tasks.Contracts.Responses;
using Task = TaskManagement.Common.Models.Task;

namespace TaskManagement.Api.Features.Tasks.Endpoints;

public static class CreateTaskEnpoint
{
    public const string Name = "CreateTask";

    public static IEndpointRouteBuilder MapCreateTask(this IEndpointRouteBuilder app)
    {
        app.MapPost(ApiEndpoints.Tasks.Create, async ([FromBody] CreateTaskRequest request,
                [FromServices]IMediator mediator, 
                [FromServices]ILogger<ILogger> logger,
                CancellationToken token) =>
            {
                var command = request.MapToCommand();
                var createTaskResult = await mediator.SendAsync<CreateTaskCommand,Result<Task>>(command, token);
                if (createTaskResult.IsFailure)
                {
                    logger.LogError(string.Join(", ", createTaskResult.Errors.Select(e => e.Message)));
                    return Results.Problem( "An error occurred while creating the task");
                }
                
                var response = new Task(command.Id, 
                    command.Title,
                    command.Description,
                    command.Deadline,
                    command.Priority,
                    command.Status,
                    command.UserId);
                return TypedResults.CreatedAtRoute(response, $"{CreateTaskEnpoint.Name}V1", new { id = command.Id });
            })
            .WithName($"{Name}V1")
            .Produces<TaskResponse>(StatusCodes.Status201Created)
            .WithApiVersionSet(ApiVersioning.VersionSet)
            .HasApiVersion(1.0);
        
    
        return app;
    }
}