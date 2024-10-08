using Microsoft.AspNetCore.Mvc;
using TaskManagement.Api.Auth;
using TaskManagement.Api.Features.Tasks.Mapping;
using TaskManagement.Common.Middleware;
using TaskManagement.Common.ResultPattern;
using TaskManagement.Common.ResultPattern.Errors;
using TaskManagement.Tasks.Commands.UpdateTask;
using TaskManagement.Tasks.Contracts.Requests;
using TaskManagement.Tasks.Contracts.Responses;
using Task = TaskManagement.Common.Models.Task;

namespace TaskManagement.Api.Features.Tasks.Endpoints;

public static class UpdateTaskEndpoint
{
    private const string Name = "UpdateTask";

    public static IEndpointRouteBuilder MapUpdateTask(this IEndpointRouteBuilder app)
    {
        app.MapPut(ApiEndpoints.Tasks.Update, async (Guid id,
                [FromBody]UpdateTaskRequest request, 
                [FromServices] IMediator mediator,
                [FromServices]ILogger<ILogger> logger,
                CancellationToken token) =>
            {
                var command = request.MapToCommand(id);
                var updateTaskResult = await mediator.SendAsync<UpdateTaskCommand, Result<Task>>(command, token);
                if (updateTaskResult.IsFailure && updateTaskResult.Errors.OfType<NotFoundError>().Any())
                {
                    return Results.NotFound(new { Message = "Task not found" });
                }

                if (updateTaskResult.IsFailure)
                {
                    logger.LogError(string.Join(", ", updateTaskResult.Errors.Select(e => e.Message)));
                    return Results.Problem( "An error occurred while deleting the task");
                }
                
                var response = updateTaskResult.Response.MapToResponse();
                return TypedResults.Ok(response);
            })
            .WithName(Name)
            .Produces<TaskResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .RequireAuthorization(AuthConstants.TrustedMemberPolicyName);
        return app;
    }
}