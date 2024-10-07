using Microsoft.AspNetCore.Mvc;
using TaskManagement.Api.Features.Users.Mapping;
using TaskManagement.Common.Middleware;
using TaskManagement.Common.Models;
using TaskManagement.Common.ResultPattern;
using TaskManagement.Users.Commands.GetAllUsers;
using TaskManagement.Users.Contracts.Requests;
using TaskManagement.Users.Contracts.Responses;

namespace TaskManagement.Api.Features.Users.Endpoints;

public static class GetAllUsersEndpoint
{
    public const string Name = "GetAllUsers";

    public static IEndpointRouteBuilder MapGetAllUsers(this IEndpointRouteBuilder app)
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
            .WithName($"{Name}V1")
            .Produces<UsersResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .WithApiVersionSet(ApiVersioning.VersionSet)
            .HasApiVersion(1.0);
        return app;
    }
}