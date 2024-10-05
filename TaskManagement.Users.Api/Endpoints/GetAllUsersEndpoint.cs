using Microsoft.AspNetCore.Mvc;
using TaskManagement.Users.Api.Mapping;
using TaskManagement.Users.Contracts.Requests;
using TaskManagement.Users.Contracts.Responses;
using TaskManagement.Users.Services;

namespace TaskManagement.Users.Api.Endpoints;

public static class GetAllUsersEndpoint
{
    public const string Name = "GetAll";

    public static IEndpointRouteBuilder MapGetAllUsers(this IEndpointRouteBuilder app)
    {
        app.MapGet(ApiEndpoints.Users.GetAll, async ([AsParameters] GetAllUsersRequest request, 
                [FromServices] IUserService userService, CancellationToken token) =>
            {
                var options = request.MapToGetAllUsersOptions();
                var users = await userService.GetAllAsync(options, token);
                if (!users.Any())
                {
                    return Results.NotFound();
                }

                var response = users.MapToResponse();
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