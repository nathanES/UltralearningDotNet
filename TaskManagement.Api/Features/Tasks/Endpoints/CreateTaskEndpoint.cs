using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using TaskManagement.Api.Auth;
using TaskManagement.Api.Cache;
using TaskManagement.Api.Features.Tasks.Mapping;
using TaskManagement.Api.Versioning;
using TaskManagement.Common.Middleware;
using TaskManagement.Common.ResultPattern;
using TaskManagement.Tasks.Commands.CreateTask;
using TaskManagement.Tasks.Contracts.Requests;
using TaskManagement.Tasks.Contracts.Responses;
using Task = TaskManagement.Common.Models.Task;

namespace TaskManagement.Api.Features.Tasks.Endpoints;

public static class CreateTaskEndpoint
{
    public const string Name = "CreateTask";
    public static IEndpointRouteBuilder MapCreateTask(this IEndpointRouteBuilder app)
    {
        app.MapPost(ApiEndpoints.Tasks.Create, async ([FromBody] CreateTaskRequest request,
                [FromServices] IMediator mediator,
                [FromServices] ILogger<ILogger> logger,
                [FromServices] IOutputCacheStore outputCacheStore,
                CancellationToken token) =>
            {
                var command = request.MapToCommand();
                var createTaskResult = await mediator.SendAsync<CreateTaskCommand, Result<Task>>(command, token);
                if (createTaskResult.IsFailure)
                {
                    logger.LogError(string.Join(", ", createTaskResult.Errors.Select(e => e.Message)));
                    return Results.Problem("An error occurred while creating the task");
                }
                
                await outputCacheStore.EvictByTagAsync(PolicyConstants.GetAllTasksCache.tag, token);
                await outputCacheStore.EvictByTagAsync(PolicyConstants.GetTaskCache.tag, token);
                var response = createTaskResult.Response.MapToResponse();
                return TypedResults.CreatedAtRoute(response, $"{CreateTaskEndpoint.Name}V1", new { id = response.Id });
            })
            .WithName($"{Name}V1")
            .Produces<TaskResponse>(StatusCodes.Status201Created)
            .WithApiVersionSet(ApiVersioning.VersionSet)
            .HasApiVersion(1.0)
            // .RequireAuthorization(AuthConstants.TrustedMemberPolicyName)
            ;


        return app;
    }
}