using Microsoft.AspNetCore.Authorization;

namespace TaskManagement.Api.Health;

public static class HealthExtension
{
    public static IApplicationBuilder AddHealthEndpoints(this IApplicationBuilder app)
    {
        //app.UseRouting(); It should be called before 
        
        //We don't see Health Endpoints in Swagger, there is no support from microsoft
        app.UseEndpoints(endpoints => { endpoints.MapHealthChecks("/health").WithMetadata(new AllowAnonymousAttribute()); });

        return app;
    }
}