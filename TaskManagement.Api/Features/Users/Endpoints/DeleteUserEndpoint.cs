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
using TaskManagement.Users.Commands.DeleteUser;

namespace TaskManagement.Api.Features.Users.Endpoints;

public static class DeleteUserEndpoint
{
    private const string Name = "DeleteUser";

    public static IEndpointRouteBuilder MapDeleteUserOutputCache(this IEndpointRouteBuilder app)
    {
        app.MapDelete(ApiEndpoints.Users.Delete, async (Guid id,
                [FromServices] IMediator mediator,
                [FromServices]ILogger<ILogger> logger,
                IOutputCacheStore outputCacheStore,
                CancellationToken token) =>
            {
                var command = new DeleteUserCommand(id);
                var deleteUserResult = await mediator.SendAsync<DeleteUserCommand, Result<None>>(command, token);
                if (deleteUserResult.IsFailure && deleteUserResult.Errors.OfType<NotFoundError>().Any())
                {
                    return Results.NotFound(new { Message = "User not found" });
                }

                if (deleteUserResult.IsFailure)
                {
                    logger.LogError(string.Join(", ", deleteUserResult.Errors.Select(e => e.Message)));
                    return Results.Problem( "An error occurred while deleting the user");
                }
                await outputCacheStore.EvictByTagAsync(PolicyConstants.GetUserCache.tag, token);
                await outputCacheStore.EvictByTagAsync(PolicyConstants.GetAllUsersCache.tag, token);
                return Results.Ok();
            })
            .WithName(Name)
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .RequireAuthorization(AuthConstants.TrustedMemberPolicyName);
        return app;
    }
    
    public static IEndpointRouteBuilder MapDeleteUserRedisCache(this IEndpointRouteBuilder app,
        IConfiguration config)
    {
        app.MapDelete(ApiEndpoints.Users.Delete, async (Guid id,
                [FromServices]IMediator mediator,
                [FromServices]ILogger<ILogger> logger,
                [FromServices]ICacheService cacheService,
                CancellationToken token) =>
            {
                var command = new DeleteUserCommand(id);
                var deleteUserResult = await mediator.SendAsync<DeleteUserCommand, Result<None>>(command, token);
                if (deleteUserResult.IsFailure && deleteUserResult.Errors.OfType<NotFoundError>().Any())
                {
                    return Results.NotFound(new { Message = "User not found" });
                }

                if (deleteUserResult.IsFailure)
                {
                    logger.LogError(string.Join(", ", deleteUserResult.Errors.Select(e => e.Message)));
                    return Results.Problem( "An error occurred while deleting the user");
                }
                await cacheService.InvalidateCacheWithTag(config["Redis:Tags:GetUser"]!, token);
                await cacheService.InvalidateCacheWithTag(config["Redis:Tags:GetAllUsers"]!, token); 
                return Results.Ok();
            })
            .WithName(Name)
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .RequireAuthorization(AuthConstants.TrustedMemberPolicyName);
        return app;
    }
}