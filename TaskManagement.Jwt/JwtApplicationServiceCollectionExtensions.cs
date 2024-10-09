using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using TaskManagement.Common.Middleware;
using TaskManagement.Common.ResultPattern;
using TaskManagement.Jwt.Commands.CreateJwt;
using TaskManagement.Jwt.Validators;

namespace TaskManagement.Jwt;

public static class JwtApplicationServiceCollectionExtensions
{
    public static IServiceCollection AddJwtApplication(this IServiceCollection services)
    {
        services.AddScoped<IRequestHandler<CreateJwtCommand, Result<string>>, CreateJwtHandler>();

        services.AddValidatorsFromAssemblyContaining<IValidatorMarker>(ServiceLifetime.Transient);
        return services;
    } 
}