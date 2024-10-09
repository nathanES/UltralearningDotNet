using Microsoft.AspNetCore.Mvc;
using TaskManagement.Api.Features.JWT.Mapping;
using TaskManagement.Api.Versioning;
using TaskManagement.Common.Middleware;
using TaskManagement.Common.ResultPattern;
using TaskManagement.Jwt.Commands.CreateJwt;
using TaskManagement.Jwt.Contracts.Requests;
using TaskManagement.Jwt.Contracts.Responses;

namespace TaskManagement.Api.Features.JWT.Endpoints;

public static class CreateJwtEndpoint
{
    public const string Name = "CreateJwt";

    public static IEndpointRouteBuilder MapCreateJwt(this IEndpointRouteBuilder app)
    {
        app.MapPost(ApiEndpoints.Jwt.Create, async ([FromBody] CreateJwtRequest request,
                [FromServices]IMediator mediator, 
                [FromServices]ILogger<ILogger> logger,
                CancellationToken token) =>
            {
                var command = request.MapToCommand();
                var createJwtResult = await mediator.SendAsync<CreateJwtCommand, Result<string>>(command, token);
                if (createJwtResult.IsFailure)
                {
                    logger.LogError("Error creating JWT for UserId {UserId}: {Errors}", request.UserId, string.Join(", ", createJwtResult.Errors.Select(e => e.Message)));
                    return Results.Problem( "An error occurred while creating the jwt");
                }

                JwtResponse response = new JwtResponse(request.UserId, createJwtResult.Response);
                return TypedResults.CreatedAtRoute(response, $"{CreateJwtEndpoint.Name}V1", new { id = response.Id });
            })
            .WithName($"{Name}V1")
            .Produces<JwtResponse>(StatusCodes.Status201Created)
            .WithApiVersionSet(ApiVersioning.VersionSet)
            .HasApiVersion(1.0);
        
    
        return app;
    }
}