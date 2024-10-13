using TaskManagement.Jwt.Configurations;
using TaskManagement.Jwt;

namespace TaskManagement.Api.Features.JWT;

public static class JwtExtensions
{
    public static IServiceCollection AddJwtServices(this IServiceCollection services, ConfigurationManager config)
    {
        services.Configure<JwtSettings>(config.GetSection("Jwt"));
        services.AddJwtApplication();
        return services;
    } 
}