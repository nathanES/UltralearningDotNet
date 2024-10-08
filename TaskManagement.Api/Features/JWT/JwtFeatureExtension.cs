using TaskManagement.Jwt.Configurations;
using TaskManagement.Jwt;

namespace TaskManagement.Api.Features.JWT;

public static class JwtFeatureExtension
{
    public static IServiceCollection AddJwtFeatureServices(this IServiceCollection services, ConfigurationManager config)
    {
        services.Configure<JwtSettings>(config.GetSection("Jwt"));
        services.AddJwtApplication();
        return services;
    } 
}