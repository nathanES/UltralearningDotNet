using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using TaskManagement.Api.Auth;
using TaskManagement.Api.Cache;
using TaskManagement.Api.Cache.OutputCache;
using TaskManagement.Api.Cache.Redis;
using TaskManagement.Api.Features.Users.Mapping;
using TaskManagement.Common.Middleware;
using TaskManagement.Common.Models;
using TaskManagement.Common.ResultPattern;
using TaskManagement.Common.ResultPattern.Errors;
using TaskManagement.Tasks.Contracts.Responses;
using TaskManagement.Users.Commands.GetUser;
using TaskManagement.Users.Contracts.Responses;
using Task = System.Threading.Tasks.Task;

namespace TaskManagement.Api.Features.Users.Endpoints;

public static class GetUserEndpoint
{
    public const string Name = "GetUser";

    public static IEndpointRouteBuilder MapGetUserOutputCache(this IEndpointRouteBuilder app)
    {
        app.MapGet(ApiEndpoints.Users.Get, async (Guid id,
                [FromServices] IMediator mediator,
                [FromServices] ILogger<ILogger> logger,
                CancellationToken token) =>
            {
                var command = new GetUserCommand(id);
                var getUserResult = await mediator.SendAsync<GetUserCommand, Result<User>>(command, token);
                if (getUserResult.IsFailure && getUserResult.Errors.OfType<NotFoundError>().Any())
                {
                    return Results.NotFound(new { Message = "User not found" });
                }

                if (getUserResult.IsFailure)
                {
                    logger.LogError(string.Join(", ", getUserResult.Errors.Select(e => e.Message)));
                    return Results.Problem("An error occurred while retrieving the user");
                }

                var response = getUserResult.Response.MapToResponse();
                return TypedResults.Ok(response);
            })
            .CacheOutput(PolicyConstants.GetUserCache.name)
            .WithName(Name)
            .Produces<UserResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);
        return app;
    }

    public static IEndpointRouteBuilder MapGetUserRedisCache(this IEndpointRouteBuilder app,
        IConfiguration config)
    {
        app.MapGet(ApiEndpoints.Users.Get, async (Guid id,
                [FromServices] IMediator mediator,
                [FromServices] ILogger<ILogger> logger,
                [FromServices] ICacheService cacheService,
                CancellationToken token) =>
            {
                
                var cacheKey = config["Redis:CacheKey:GetUser"]!.Replace("{Id}", id.ToString()); 
                var getFromTaskResult = await cacheService.GetFromCache(cacheKey, token);
                if (getFromTaskResult.IsSuccess)
                {
                    return TypedResults.Ok(getFromTaskResult.Response);
                } 

                var command = new GetUserCommand(id);
                var getUserResult = await mediator.SendAsync<GetUserCommand, Result<User>>(command, token);
                if (getUserResult.IsFailure && getUserResult.Errors.OfType<NotFoundError>().Any())
                {
                    return Results.NotFound(new { Message = "User not found" });
                }

                if (getUserResult.IsFailure)
                {
                    logger.LogError(string.Join(", ", getUserResult.Errors.Select(e => e.Message)));
                    return Results.Problem("An error occurred while retrieving the user");
                }

                var response = getUserResult.Response.MapToResponse();
                await cacheService.AddToCache(cacheKey, JsonConvert.SerializeObject(response),
                    config["Redis:Tags:GetUser"]!, token);
                return TypedResults.Ok(response);
            })
            .WithName(Name)
            .Produces<UserResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);
        return app;
    }
}