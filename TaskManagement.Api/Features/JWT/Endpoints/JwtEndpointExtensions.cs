namespace TaskManagement.Api.Features.JWT.Endpoints;

public static class JwtEndpointExtensions
{
    public static IEndpointRouteBuilder AddJwtEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapCreateJwt();
        return app;
    } 
}