using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using TaskManagement.Api.Auth;
using TaskManagement.Api.Cache;
using TaskManagement.Api.Cache.OutputCache;
using TaskManagement.Api.Cache.Redis;
using TaskManagement.Api.Features.Users.Mapping;
using TaskManagement.Api.Versioning;
using TaskManagement.Common.Middleware;
using TaskManagement.Common.Models;
using TaskManagement.Common.ResultPattern;
using TaskManagement.Tasks.Contracts.Responses;
using TaskManagement.Users.Commands.GetAllUsers;
using TaskManagement.Users.Contracts.Requests;
using TaskManagement.Users.Contracts.Responses;
using Task = System.Threading.Tasks.Task;

namespace TaskManagement.Api.Features.Users.Endpoints;

public static class GetAllUsersEndpoint
{
    public const string Name = "GetAllUsers";

    public static IEndpointRouteBuilder MapGetAllUsersOutputCache(this IEndpointRouteBuilder app)
    {
        app.MapGet(ApiEndpoints.Users.GetAll, async ([AsParameters] GetAllUsersRequest request, 
                [FromServices] IMediator mediator, 
                [FromServices]ILogger<ILogger> logger,
                CancellationToken token) =>
            {
                var command = request.MapToCommand();
                var getUsersResult = await mediator.SendAsync<GetAllUsersCommand, Result<IEnumerable<User>>>(command, token);
                if (getUsersResult.IsFailure)
                {
                    logger.LogError(string.Join(", ", getUsersResult.Errors.Select(e => e.Message)));
                    return Results.Problem( "An error occurred while retrieving all users"); 
                } 

                var response = getUsersResult.Response.MapToResponse();
                return TypedResults.Ok(response);
            })
            .CacheOutput(PolicyConstants.GetAllUsersCache.name)
            .WithName($"{Name}V1")
            .Produces<UsersResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .WithApiVersionSet(ApiVersioning.VersionSet)
            .HasApiVersion(1.0);
        return app;
    }
    public static IEndpointRouteBuilder MapGetAllUsersRedisCache(this IEndpointRouteBuilder app,
        IConfiguration config)
    {
        app.MapGet(ApiEndpoints.Users.GetAll, async ([AsParameters] GetAllUsersRequest request, 
                [FromServices] IMediator mediator, 
                [FromServices]ILogger<ILogger> logger,
                [FromServices] ICacheService cacheService,
                CancellationToken token) =>
            {
                var cacheKey = GenerateCacheKey(config["Redis:CacheKey:GetAllUsers"]!, request);
                var getFromCacheResult = await cacheService.GetFromCache(cacheKey, token);
                if (getFromCacheResult.IsSuccess)
                {
                    return TypedResults.Ok(getFromCacheResult.Response);
                }
                
                var command = request.MapToCommand();
                var getUsersResult = await mediator.SendAsync<GetAllUsersCommand, Result<IEnumerable<User>>>(command, token);
                if (getUsersResult.IsFailure)
                {
                    logger.LogError(string.Join(", ", getUsersResult.Errors.Select(e => e.Message)));
                    return Results.Problem( "An error occurred while retrieving all users"); 
                } 

                var response = getUsersResult.Response.MapToResponse();
                await cacheService.AddToCache(cacheKey, JsonConvert.SerializeObject(response),
                    config["Redis:Tags:GetAllUsers"]!, token);
                return TypedResults.Ok(response);
            })
            .WithName($"{Name}V1")
            .Produces<UsersResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .WithApiVersionSet(ApiVersioning.VersionSet)
            .HasApiVersion(1.0);
        return app;
    }

    static string GenerateCacheKey(string cacheKeyTemplate, GetAllUsersRequest request)
    {
        if (string.IsNullOrEmpty(cacheKeyTemplate))
        {
            throw new InvalidOperationException("Cache key template not found in configuration.");
        } 
        return cacheKeyTemplate
            .Replace("{Page}", (request.Page, PagedRequest.DefaultPage).ToString())
            .Replace("{PageSize}", (request.PageSize ?? PagedRequest.DefaultPageSize).ToString())
            .Replace("{Username}", request.Username ?? "null")
            .Replace("{Email}", request.Email ?? "null");
    }
    
    
}