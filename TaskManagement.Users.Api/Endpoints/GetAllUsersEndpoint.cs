using Microsoft.AspNetCore.Mvc;
using TaskManagement.Common.Mediator;
using TaskManagement.Common.Models;
using TaskManagement.Users.Api.Mapping;
using TaskManagement.Users.Commands.GetAllUsers;
using TaskManagement.Users.Contracts.Requests;
using TaskManagement.Users.Contracts.Responses;
using TaskManagement.Users.Interfaces;
using TaskManagement.Users.Services;

namespace TaskManagement.Users.Api.Endpoints;

public static class GetAllUsersEndpoint
{
    public const string Name = "GetAll";

    public static IEndpointRouteBuilder MapGetAllUsers(this IEndpointRouteBuilder app)
    {
        app.MapGet(ApiEndpoints.Users.GetAll, async ([AsParameters] GetAllUsersRequest request, 
                [FromServices] Mediator mediator, CancellationToken token) =>
            {
                var command = request.MapToCommand();
                var users = await mediator.SendAsync<GetAllUsersCommand, IEnumerable<User>>(command, token);
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