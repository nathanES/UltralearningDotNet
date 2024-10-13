namespace TaskManagement.Api.Features.JWT.Endpoints;

public static class JwtEndpointsExtensions
{
    public static IEndpointRouteBuilder AddJwtEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapCreateJwt();
        return app;
    } 
}