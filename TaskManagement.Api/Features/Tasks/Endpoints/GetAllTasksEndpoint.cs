using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using TaskManagement.Api.Auth;
using TaskManagement.Api.Cache;
using TaskManagement.Api.Cache.OutputCache;
using TaskManagement.Api.Cache.Redis;
using TaskManagement.Api.Features.Tasks.Mapping;
using TaskManagement.Api.Versioning;
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

    public static IEndpointRouteBuilder MapGetAllTasksOutputCache(this IEndpointRouteBuilder app)
    {
        app.MapGet(ApiEndpoints.Tasks.GetAll, async ([AsParameters] GetAllTasksRequest request,
                [FromServices] IMediator mediator,
                [FromServices] ILogger<ILogger> logger,
                CancellationToken token) =>
            {
                logger.LogDebug("Requesting GetAllTasks with caching policy applied.");
                var command = request.MapToCommand();
                var getTasksResult =
                    await mediator.SendAsync<GetAllTasksCommand, Result<IEnumerable<Task>>>(command, token);
                if (getTasksResult.IsFailure)
                {
                    logger.LogError(string.Join(", ", getTasksResult.Errors.Select(e => e.Message)));
                    return Results.Problem("An error occurred while retrieving all tasks");
                }

                var response = getTasksResult.Response.MapToResponse();
                return TypedResults.Ok(response);
            })
            .CacheOutput(PolicyConstants.GetAllTasksCache.name)
            .WithName($"{Name}V1")
            .Produces<TasksResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .WithApiVersionSet(ApiVersioning.VersionSet)
            .HasApiVersion(1.0);
        return app;
    }

    public static IEndpointRouteBuilder MapGetAllTasksRedisCache(this IEndpointRouteBuilder app,
        IConfiguration config)
    {
        app.MapGet(ApiEndpoints.Tasks.GetAll, async ([AsParameters] GetAllTasksRequest request,
                [FromServices] IMediator mediator,
                [FromServices] ILogger<ILogger> logger,
                [FromServices] ICacheService cacheService,
                CancellationToken token) =>
            {
                var cacheKey = GenerateCacheKey(config["Redis:CacheKey:GetAllTasks"]!, request);
                var getFromCacheResult = await cacheService.GetFromCache(cacheKey, token);
                if (getFromCacheResult.IsSuccess)
                {
                    return TypedResults.Ok(getFromCacheResult.Response);
                }
             

                var command = request.MapToCommand();
                var getTasksResult =
                    await mediator.SendAsync<GetAllTasksCommand, Result<IEnumerable<Task>>>(command, token);

                if (getTasksResult.IsFailure)
                {
                    logger.LogError(string.Join(", ", getTasksResult.Errors.Select(e => e.Message)));
                    return Results.Problem("An error occurred while retrieving tasks");
                }

                var response = getTasksResult.Response.MapToResponse();
                await cacheService.AddToCache(cacheKey, JsonConvert.SerializeObject(response),
                    config["Redis:Tags:GetAllTasks"]!, token);
                return TypedResults.Ok(response);
            })
            .WithName($"{Name}V1")
            .Produces<TasksResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .WithApiVersionSet(ApiVersioning.VersionSet)
            .HasApiVersion(1.0);
        return app;
    }

    private static string GenerateCacheKey(string cacheKeyTemplate, GetAllTasksRequest request)
    {
        if (string.IsNullOrEmpty(cacheKeyTemplate))
        {
            throw new InvalidOperationException("Cache key template not found in configuration.");
        }
        
        return cacheKeyTemplate
            .Replace("{Page}", (request.Page ?? PagedRequest.DefaultPage).ToString())
            .Replace("{PageSize}", (request.PageSize ?? PagedRequest.DefaultPageSize).ToString())
            .Replace("{Title}", request.Title ?? "null")
            .Replace("{Description}", request.Description ?? "null")
            .Replace("{Deadline}", request.Deadline?.ToString("yyyyMMdd") ?? "null")
            .Replace("{Priority}", request.Priority?.ToString() ?? "null")
            .Replace("{Status}", request.Status?.ToString() ?? "null");
    }
}