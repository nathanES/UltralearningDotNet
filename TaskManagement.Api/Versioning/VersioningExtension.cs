using Asp.Versioning;

namespace TaskManagement.Api.Versioning;

public static class VersioningExtension
{
    public static IServiceCollection AddVersioning(this IServiceCollection services)
    {
        services.AddApiVersioning(x =>
        {
            x.DefaultApiVersion = new ApiVersion(1.0);
            x.AssumeDefaultVersionWhenUnspecified = true;
            x.ReportApiVersions = true;
            x.ApiVersionReader = new MediaTypeApiVersionReader("api-version");
            //Header of the request : "Accept" : "application/json;api-version=1.0"
        }).AddApiExplorer();
        return services;
    }    
}