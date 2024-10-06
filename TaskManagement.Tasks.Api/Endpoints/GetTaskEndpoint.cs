using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using TaskManagement.Common.Mediator;
using TaskManagement.Tasks.Api.Mapping;
using TaskManagement.Tasks.Commands.GetTask;
using TaskManagement.Tasks.Contracts.Responses;
using TaskManagement.Tasks.Interfaces;
using TaskManagement.Tasks.Services;
using Task = TaskManagement.Common.Models.Task;


namespace TaskManagement.Tasks.Api.Endpoints;

public static class GetTaskEndpoint
{
    public const string Name = "GetTask";

    public static IEndpointRouteBuilder MapGetTask(this IEndpointRouteBuilder app)
    {
        app.MapGet(ApiEndpoints.Tasks.Get, async (Guid id,
                [FromServices] Mediator mediator, CancellationToken token) =>
            {
                var command = new GetTaskCommand(id);
                var task = await mediator.SendAsync<GetTaskCommand, Task?>(command);
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