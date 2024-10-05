using Microsoft.AspNetCore.Mvc;
using TaskManagement.Users.Services;

namespace TaskManagement.Users.Api.Endpoints;

public static class DeleteUserEndpoint
{
    private const string Name = "DeleteUser";

    public static IEndpointRouteBuilder MapDeleteUser(this IEndpointRouteBuilder app)
    {
        app.MapDelete(ApiEndpoints.Users.Delete, async (Guid id,
                [FromServices] IUserService userService, CancellationToken token) =>
            {
                var isDeleted = await userService.DeleteByIdAsync(id, token);
                if (!isDeleted)
                {
                    return Results.NotFound();
                }

                return Results.Ok();
            })
            .WithName(Name)
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);
        return app;
    }
}