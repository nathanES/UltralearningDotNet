using System.Data.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using TaskManagement.Common.Mediator;
using TaskManagement.Tasks.Api.Mapping;
using TaskManagement.Tasks.Commands.CreateTask;
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
                [FromServices]Mediator mediator, CancellationToken token) =>
            {
                var command = request.MapToCommand();
                await mediator.SendAsync<CreateTaskCommand,bool>(command, token);
                var response = new TaskManagement.Common.Models.Task(command.Id, 
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