using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using TaskManagement.Api.Auth;
using TaskManagement.Api.Cache;
using TaskManagement.Api.Cache.OutputCache;
using TaskManagement.Api.Cache.Redis;
using TaskManagement.Api.Features.Tasks.Mapping;
using TaskManagement.Common.Middleware;
using TaskManagement.Common.ResultPattern;
using TaskManagement.Common.ResultPattern.Errors;
using TaskManagement.Tasks.Commands.GetTask;
using TaskManagement.Tasks.Contracts.Responses;
using Task = TaskManagement.Common.Models.Task;


namespace TaskManagement.Api.Features.Tasks.Endpoints;

public static class GetTaskEndpoint
{
    public const string Name = "GetTask";

    public static IEndpointRouteBuilder MapGetTaskOutputCache(this IEndpointRouteBuilder app)
    {
        app.MapGet(ApiEndpoints.Tasks.Get, async (Guid id,
                [FromServices] IMediator mediator,
                [FromServices] ILogger<ILogger> logger,
                CancellationToken token) =>
            {
                var command = new GetTaskCommand(id);
                var getTaskResult = await mediator.SendAsync<GetTaskCommand, Result<Task>>(command, token);
                if (getTaskResult.IsFailure && getTaskResult.Errors.OfType<NotFoundError>().Any())
                {
                    return Results.NotFound(new { Message = "Task not found" });
                }

                if (getTaskResult.IsFailure)
                {
                    logger.LogError(string.Join(", ", getTaskResult.Errors.Select(e => e.Message)));
                    return Results.Problem("An error occurred while retrieving the task");
                }

                var response = getTaskResult.Response.MapToResponse();
                return TypedResults.Ok(response);
            })
            .CacheOutput(PolicyConstants.GetTaskCache.name)
            .WithName(Name)
            .Produces<TaskResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .RequireAuthorization(AuthConstants.TrustedMemberPolicyName);
        return app;
    }

    public static IEndpointRouteBuilder MapGetTaskRedisCache(this IEndpointRouteBuilder app,
        IConfiguration config)
    {
        app.MapGet(ApiEndpoints.Tasks.Get, async (Guid id, 
                [FromServices] IMediator mediator,
                [FromServices] ILogger<ILogger> logger,
                [FromServices] ICacheService cacheService,
                CancellationToken token) =>
            {
                var cacheKey = config["Redis:CacheKey:GetTask"]!.Replace("{Id}", id.ToString());
                var getFromTaskResult = await cacheService.GetFromCache(cacheKey, token);
                if (getFromTaskResult.IsSuccess)
                {
                    return TypedResults.Ok(getFromTaskResult.Response);
                }

                var command = new GetTaskCommand(id);
                var getTaskResult = await mediator.SendAsync<GetTaskCommand, Result<Task>>(command, token);
                if (getTaskResult.IsFailure && getTaskResult.Errors.OfType<NotFoundError>().Any())
                {
                    return Results.NotFound(new { Message = "Task not found" });
                }

                if (getTaskResult.IsFailure)
                {
                    logger.LogError(string.Join(", ", getTaskResult.Errors.Select(e => e.Message)));
                    return Results.Problem("An error occurred while retrieving the task");
                }

                var response = getTaskResult.Response.MapToResponse();
                await cacheService.AddToCache(cacheKey, JsonConvert.SerializeObject(response),
                    config["Redis:Tags:GetTask"]!, token);
                return TypedResults.Ok(response);
            })
            .WithName(Name)
            .Produces<TaskResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .RequireAuthorization(AuthConstants.TrustedMemberPolicyName);
        return app;
    }
}