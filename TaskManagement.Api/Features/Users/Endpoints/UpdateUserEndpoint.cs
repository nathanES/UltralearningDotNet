using Microsoft.AspNetCore.Mvc;
using TaskManagement.Api.Features.Users.Mapping;
using TaskManagement.Common.Middleware;
using TaskManagement.Common.Models;
using TaskManagement.Common.ResultPattern;
using TaskManagement.Common.ResultPattern.Errors;
using TaskManagement.Users.Commands.UpdateUser;
using TaskManagement.Users.Contracts.Requests;
using TaskManagement.Users.Contracts.Responses;

namespace TaskManagement.Api.Features.Users.Endpoints;

public static class UpdateUserEndpoint
{
    private const string Name = "UpdateUser";

    public static IEndpointRouteBuilder MapUpdateUser(this IEndpointRouteBuilder app)
    {
        app.MapPut(ApiEndpoints.Users.Update, async (Guid id,
                [FromBody]UpdateUserRequest request, 
                [FromServices] IMediator mediator, [FromServices]ILogger<ILogger> logger,
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
                    return Results.Problem( "An error occurred while deleting the user");
                }
                var response = updateUserResult.Response.MapToResponse();
                return TypedResults.Ok(response);
            })
            .WithName(Name)
            .Produces<UserResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);
        return app;
    }
}