using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.Extensions.Caching.Distributed;
using StackExchange.Redis;
using TaskManagement.Api.Auth;
using TaskManagement.Api.Cache;
using TaskManagement.Api.Cache.OutputCache;
using TaskManagement.Api.Cache.Redis;
using TaskManagement.Common.Middleware;
using TaskManagement.Common.ResultPattern;
using TaskManagement.Common.ResultPattern.Errors;
using TaskManagement.Tasks.Commands.DeleteTask;

namespace TaskManagement.Api.Features.Tasks.Endpoints;

public static class DeleteTaskEndpoint
{
    private const string Name = "DeleteTask";

    public static IEndpointRouteBuilder MapDeleteTaskOutputCache(this IEndpointRouteBuilder app)
    {
        app.MapDelete(ApiEndpoints.Tasks.Delete, async (Guid id,
                [FromServices]IMediator mediator,
                [FromServices]ILogger<ILogger> logger,
                IOutputCacheStore outputCacheStore,
                CancellationToken token) =>
            {
                var command = new DeleteTaskCommand(id);
                var deleteTaskResult = await mediator.SendAsync<DeleteTaskCommand, Result<None>>(command, token);
                if (deleteTaskResult.IsFailure && deleteTaskResult.Errors.OfType<NotFoundError>().Any())
                {
                    return Results.NotFound(new { Message = "Task not found" });
                }

                if (deleteTaskResult.IsFailure)
                {
                    logger.LogError(string.Join(", ", deleteTaskResult.Errors.Select(e => e.Message)));
                    return Results.Problem( "An error occurred while deleting the task");
                }

                await outputCacheStore.EvictByTagAsync(PolicyConstants.GetAllTasksCache.tag, token);
                await outputCacheStore.EvictByTagAsync(PolicyConstants.GetTaskCache.tag, token);
                return Results.Ok();
            })
            .WithName(Name)
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .RequireAuthorization(AuthConstants.TrustedMemberPolicyName);
        return app;
    }
    public static IEndpointRouteBuilder MapDeleteTaskRedisCache(this IEndpointRouteBuilder app,
        IConfiguration config)
    {
        app.MapDelete(ApiEndpoints.Tasks.Delete, async (Guid id,
                [FromServices]IMediator mediator,
                [FromServices]ILogger<ILogger> logger,
                [FromServices]ICacheService cacheService,
                CancellationToken token) =>
            {
                var command = new DeleteTaskCommand(id);
                var deleteTaskResult = await mediator.SendAsync<DeleteTaskCommand, Result<None>>(command, token);
                if (deleteTaskResult.IsFailure && deleteTaskResult.Errors.OfType<NotFoundError>().Any())
                {
                    return Results.NotFound(new { Message = "Task not found" });
                }

                if (deleteTaskResult.IsFailure)
                {
                    logger.LogError(string.Join(", ", deleteTaskResult.Errors.Select(e => e.Message)));
                    return Results.Problem( "An error occurred while deleting the task");
                }

                await cacheService.InvalidateCacheWithTag(config["Redis:Tags:GetTask"]!, token);
                await cacheService.InvalidateCacheWithTag(config["Redis:Tags:GetAllTasks"]!, token); 
                return Results.Ok();
            })
            .WithName(Name)
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .RequireAuthorization(AuthConstants.TrustedMemberPolicyName);
        return app;
    }
}