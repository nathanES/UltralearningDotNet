using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.Extensions.Caching.Distributed;
using StackExchange.Redis;
using TaskManagement.Api.Auth;
using TaskManagement.Api.Cache;
using TaskManagement.Api.Cache.OutputCache;
using TaskManagement.Api.Cache.Redis;
using TaskManagement.Api.Features.Users.Mapping;
using TaskManagement.Common.Middleware;
using TaskManagement.Common.Models;
using TaskManagement.Common.ResultPattern;
using TaskManagement.Common.ResultPattern.Errors;
using TaskManagement.Users.Commands.UpdateUser;
using TaskManagement.Users.Contracts.Requests;
using TaskManagement.Users.Contracts.Responses;
using Task = System.Threading.Tasks.Task;

namespace TaskManagement.Api.Features.Users.Endpoints;

public static class UpdateUserEndpoint
{
    private const string Name = "UpdateUser";

    public static IEndpointRouteBuilder MapUpdateUserOutputCache(this IEndpointRouteBuilder app)
    {
        app.MapPut(ApiEndpoints.Users.Update, async (Guid id,
                [FromBody] UpdateUserRequest request,
                [FromServices] IMediator mediator,
                [FromServices] ILogger<ILogger> logger,
                IOutputCacheStore outputCacheStore,
                CancellationToken token) =>
            {
                var command = request.MapToCommand(id);
                var updateUserResult = await mediator.SendAsync<UpdateUserCommand, Result<User>>(command, token);

                if (updateUserResult.IsFailure && updateUserResult.Errors.OfType<NotFoundError>().Any())
                {
                    return Results.NotFound(new { Message = "User not found" });
                }

                if (updateUserResult.IsFailure)
                {
                    logger.LogError(string.Join(", ", updateUserResult.Errors.Select(e => e.Message)));
                    return Results.Problem("An error occurred while deleting the user");
                }

                await outputCacheStore.EvictByTagAsync(PolicyConstants.GetUserCache.tag, token);
                await outputCacheStore.EvictByTagAsync(PolicyConstants.GetAllUsersCache.tag, token);

                var response = updateUserResult.Response.MapToResponse();
                return TypedResults.Ok(response);
            })
            .WithName(Name)
            .Produces<UserResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .RequireAuthorization(AuthConstants.TrustedMemberPolicyName);
        return app;
    }
    public static IEndpointRouteBuilder MapUpdateUserRedisCache(this IEndpointRouteBuilder app,
        IConfiguration config)
    {
        app.MapPut(ApiEndpoints.Users.Update, async (Guid id,
                [FromBody]UpdateUserRequest request,
                [FromServices]IMediator mediator,
                [FromServices]ILogger<ILogger> logger,
                [FromServices]ICacheService cacheService,
                CancellationToken token) =>
            {
                var command = request.MapToCommand(id);
                var updateUserResult = await mediator.SendAsync<UpdateUserCommand, Result<User>>(command, token);

                if (updateUserResult.IsFailure && updateUserResult.Errors.OfType<NotFoundError>().Any())
                {
                    return Results.NotFound(new { Message = "User not found" });
                }

                if (updateUserResult.IsFailure)
                {
                    logger.LogError(string.Join(", ", updateUserResult.Errors.Select(e => e.Message)));
                    return Results.Problem("An error occurred while deleting the user");
                }
                await cacheService.InvalidateCacheWithTag(config["Redis:Tags:GetAllUsers"]!, token);
                await cacheService.InvalidateCacheWithTag(config["Redis:Tags:GetUser"]!, token);
                var response = updateUserResult.Response.MapToResponse();
                return TypedResults.Ok(response);
            })
            .WithName(Name)
            .Produces<UserResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .RequireAuthorization(AuthConstants.TrustedMemberPolicyName);
        return app;
    }
}