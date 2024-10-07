using Microsoft.AspNetCore.Mvc;
using TaskManagement.Api.Features.Tasks.Mapping;
using TaskManagement.Common.Middleware;
using TaskManagement.Common.ResultPattern;
using TaskManagement.Tasks.Commands.GetAllTasks;
using TaskManagement.Tasks.Contracts.Requests;
using TaskManagement.Tasks.Contracts.Responses;
using Task = TaskManagement.Common.Models.Task;

namespace TaskManagement.Api.Features.Tasks.Endpoints;

public static class GetAllTasksEndpoint
{
    public const string Name = "GetAllTasks";

    public static IEndpointRouteBuilder MapGetAllTasks(this IEndpointRouteBuilder app)
    {
        app.MapGet(ApiEndpoints.Tasks.GetAll, async ([AsParameters] GetAllTasksRequest request,
                [FromServices] IMediator mediator,
                [FromServices]ILogger<ILogger> logger,
                CancellationToken token) =>
            {
                var command = request.MapToCommand();
                var getTasksResult = await mediator.SendAsync<GetAllTasksCommand ,Result<IEnumerable<Task>>>(command, token);
                if (getTasksResult.IsFailure)
                {
                    logger.LogError(string.Join(", ", getTasksResult.Errors.Select(e => e.Message)));
                    return Results.Problem( "An error occurred while retrieving all tasks");
                }
                var response = getTasksResult.Response.MapToResponse();
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