using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TaskManagement.Common;
using TaskManagement.Common.Middleware;
using TaskManagement.Common.ResultPattern;
using TaskManagement.Jwt.Commands.CreateJwt;

namespace TaskManagement.Jwt;

public static class JwtApplicationServiceCollectionExtensions
{
    public static IServiceCollection AddJwtApplication(this IServiceCollection services)
    {
        services.AddScoped<IRequestHandler<CreateJwtCommand, Result<string>>, CreateJwtHandler>();

        // services.AddValidatorsFromAssemblyContaining<IValidatorMarker>(ServiceLifetime.Transient);
        return services;
    } 
}