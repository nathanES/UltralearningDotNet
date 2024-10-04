using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using TaskManagement.Tasks.Api.Mapping;
using TaskManagement.Tasks.Contracts.Responses;
using TaskManagement.Tasks.Services;

namespace TaskManagement.Tasks.Api.Endpoints;

public static class GetTaskEndpoint
{
    public const string Name = "GetTask";

    public static IEndpointRouteBuilder MapGetTask(this IEndpointRouteBuilder app)
    {
        app.MapGet(ApiEndpoints.Tasks.Get, async (Guid id,
                [FromServices] ITaskService taskService, CancellationToken token) =>
            {
                var task = await taskService.GetByIdAsync(id, token);
                if (task is null)
                {
                    return Results.NotFound();
                }

                var response = task?.MapToResponse();
                return TypedResults.Ok(response);
            })
            .WithName(Name)
            .Produces<TaskResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);
        return app;
    }
}