using Microsoft.AspNetCore.Mvc;
using TaskManagement.Common.Mediator;
using TaskManagement.Tasks.Api.Mapping;
using TaskManagement.Tasks.Commands.GetAllTasks;
using TaskManagement.Tasks.Contracts.Requests;
using TaskManagement.Tasks.Contracts.Responses;
using TaskManagement.Tasks.Interfaces;
using TaskManagement.Tasks.Services;
using Task = TaskManagement.Common.Models.Task;

namespace TaskManagement.Tasks.Api.Endpoints;

public static class GetAllTasksEndpoint
{
    public const string Name = "GetAll";

    public static IEndpointRouteBuilder MapGetAllTasks(this IEndpointRouteBuilder app)
    {
        app.MapGet(ApiEndpoints.Tasks.GetAll, async ([AsParameters] GetAllTasksRequest request,
                [FromServices] Mediator mediator, CancellationToken token) =>
            {
                var command = request.MapToCommand();
                var tasks = await mediator.SendAsync<GetAllTasksCommand ,IEnumerable<Task>>(command, token);
                if (!tasks.Any())
                {
                    return Results.NotFound();
                }

                var response = tasks.MapToResponse();
                return TypedResults.Ok(response);
            })
            .WithName($"{Name}V1")
            .Produces<TasksResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .WithApiVersionSet(ApiVersioning.VersionSet)
            .HasApiVersion(1.0);
        return app;
    }
}