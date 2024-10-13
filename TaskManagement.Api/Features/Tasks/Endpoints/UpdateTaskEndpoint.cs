using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.Extensions.Caching.Distributed;
using StackExchange.Redis;
using TaskManagement.Api.Auth;
using TaskManagement.Api.Cache;
using TaskManagement.Api.Cache.OutputCache;
using TaskManagement.Api.Cache.Redis;
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

    public static IEndpointRouteBuilder MapUpdateTaskOutputCache(this IEndpointRouteBuilder app)
    {
        app.MapPut(ApiEndpoints.Tasks.Update, async (Guid id,
                [FromBody] UpdateTaskRequest request,
                [FromServices] IMediator mediator,
                [FromServices] ILogger<ILogger> logger,
                IOutputCacheStore outputCacheStore,
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
                    return Results.Problem("An error occurred while deleting the task");
                }

                await outputCacheStore.EvictByTagAsync(PolicyConstants.GetAllTasksCache.tag, token);
                await outputCacheStore.EvictByTagAsync(PolicyConstants.GetTaskCache.tag, token);
                var response = updateTaskResult.Response.MapToResponse();
                return TypedResults.Ok(response);
            })
            .WithName(Name)
            .Produces<TaskResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .RequireAuthorization(AuthConstants.TrustedMemberPolicyName);
        return app;
    }

    public static IEndpointRouteBuilder MapUpdateTaskRedisCache(this IEndpointRouteBuilder app,
        IConfiguration config)
    {
        app.MapPut(ApiEndpoints.Tasks.Update, async (Guid id,
                [FromBody]UpdateTaskRequest request,
                [FromServices]IMediator mediator,
                [FromServices]ILogger<ILogger> logger,
                [FromServices]ICacheService cacheService,
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
                    return Results.Problem("An error occurred while deleting the task");
                }
                await cacheService.InvalidateCacheWithTag(config["Redis:Tags:GetAllTasks"]!, token);
                await cacheService.InvalidateCacheWithTag(config["Redis:Tags:GetTask"]!, token);
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