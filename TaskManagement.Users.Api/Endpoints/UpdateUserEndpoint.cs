using Microsoft.AspNetCore.Mvc;
using TaskManagement.Users.Api.Mapping;
using TaskManagement.Users.Contracts.Requests;
using TaskManagement.Users.Contracts.Responses;
using TaskManagement.Users.Services;

namespace TaskManagement.Users.Api.Endpoints;

public static class UpdateUserEndpoint
{
    private const string Name = "UpdateUser";

    public static IEndpointRouteBuilder MapUpdateUser(this IEndpointRouteBuilder app)
    {
        app.MapPut(ApiEndpoints.Users.Update, async (Guid id,
                [FromBody]UpdateUserRequest request, 
                [FromServices] IUserService userService, CancellationToken token) =>
            {
                var user = request.MapToUser(id);
                var updatedUser = await userService.UpdateAsync(user, token);
                if (updatedUser is null)
                {
                    return Results.NotFound();
                }
                var response = updatedUser?.MapToResponse();
                return TypedResults.Ok(response);
            })
            .WithName(Name)
            .Produces<UserResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);
        return app;
    }
}