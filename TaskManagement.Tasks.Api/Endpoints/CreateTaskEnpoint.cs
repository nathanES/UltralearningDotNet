using System.Data.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using TaskManagement.Common.Middleware;
using TaskManagement.Common.ResultPattern;
using TaskManagement.Tasks.Api.Mapping;
using TaskManagement.Tasks.Commands.CreateTask;
using TaskManagement.Tasks.Contracts.Requests;
using TaskManagement.Tasks.Contracts.Responses;
using TaskManagement.Tasks.Services;
using Task = TaskManagement.Tasks.Models.Task;

namespace TaskManagement.Tasks.Api.Endpoints;

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
                    command.DeadLine,
                    command.Priority,
                    command.Status);
                return TypedResults.CreatedAtRoute(response, $"{CreateTaskEnpoint.Name}V1", new { id = command.Id });
            })
            .WithName($"{Name}V1")
            .Produces<TaskResponse>(StatusCodes.Status201Created)
            .WithApiVersionSet(ApiVersioning.VersionSet)
            .HasApiVersion(1.0);
        
    
        return app;
    }
}