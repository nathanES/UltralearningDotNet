using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.Extensions.Caching.Distributed;
using StackExchange.Redis;
using TaskManagement.Api.Auth;
using TaskManagement.Api.Cache;
using TaskManagement.Api.Cache.OutputCache;
using TaskManagement.Api.Cache.Redis;
using TaskManagement.Api.Features.Users.Mapping;
using TaskManagement.Api.Versioning;
using TaskManagement.Common.Middleware;
using TaskManagement.Common.Models;
using TaskManagement.Common.ResultPattern;
using TaskManagement.Users.Commands.CreateUser;
using TaskManagement.Users.Contracts.Requests;
using TaskManagement.Users.Contracts.Responses;
using Task = System.Threading.Tasks.Task;

namespace TaskManagement.Api.Features.Users.Endpoints;

public static class CreateUserEndpoint
{
    public const string Name = "CreateUser";

    public static IEndpointRouteBuilder MapCreateUserOutputCache(this IEndpointRouteBuilder app)
    {
        app.MapPost(ApiEndpoints.Users.Create, async ([FromBody] CreateUserRequest request,
                [FromServices] IMediator mediator, 
                [FromServices]ILogger<ILogger> logger,
                IOutputCacheStore outputCacheStore,
                CancellationToken token) =>
            {
                var command = request.MapToCommand();
                var createUserResult = await mediator.SendAsync<CreateUserCommand, Result<User>>(command, token);
                if (createUserResult.IsFailure)
                {
                    logger.LogError(string.Join(", ", createUserResult.Errors.Select(e => e.Message)));
                    return Results.Problem( "An error occurred while creating the user");
                }

                await outputCacheStore.EvictByTagAsync(PolicyConstants.GetUserCache.tag, token);
                await outputCacheStore.EvictByTagAsync(PolicyConstants.GetAllUsersCache.tag, token);
                var response = createUserResult.Response.MapToResponse();
                return TypedResults.CreatedAtRoute(response, $"{CreateUserEndpoint.Name}V1", new { id = response.Id });
            })
            .WithName($"{Name}V1")
            .Produces<UserResponse>(StatusCodes.Status201Created)
            .WithApiVersionSet(ApiVersioning.VersionSet)
            .HasApiVersion(1.0)
            .RequireAuthorization(AuthConstants.TrustedMemberPolicyName);
        return app;
    }
    public static IEndpointRouteBuilder MapCreateUserRedisCache(this IEndpointRouteBuilder app,
        IConfiguration config)
    {
        app.MapPost(ApiEndpoints.Users.Create, async ([FromBody] CreateUserRequest request,
                [FromServices] IMediator mediator, 
                [FromServices] ILogger<ILogger> logger,
                [FromServices] ICacheService cacheService,
                CancellationToken token) =>
            {
                var command = request.MapToCommand();
                var createUserResult = await mediator.SendAsync<CreateUserCommand, Result<User>>(command, token);
                if (createUserResult.IsFailure)
                {
                    logger.LogError(string.Join(", ", createUserResult.Errors.Select(e => e.Message)));
                    return Results.Problem( "An error occurred while creating the user");
                }
                await cacheService.InvalidateCacheWithTag(config["Redis:Tags:GetAllUsers"]!, token);
                var response = createUserResult.Response.MapToResponse();
                return TypedResults.CreatedAtRoute(response, $"{CreateUserEndpoint.Name}V1", new { id = response.Id });
            })
            .WithName($"{Name}V1")
            .Produces<UserResponse>(StatusCodes.Status201Created)
            .WithApiVersionSet(ApiVersioning.VersionSet)
            .HasApiVersion(1.0)
            .RequireAuthorization(AuthConstants.TrustedMemberPolicyName);
        return app;
    }
}