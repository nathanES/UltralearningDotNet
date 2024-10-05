using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using TaskManagement.Users.Services;
using TaskManagement.Users.Validators;

namespace TaskManagement.Users;

public static class UserApplicationServiceCollectionExtensions
{
    public static IServiceCollection AddUserApplication(this IServiceCollection services)
    {
        services.AddScoped<IUserService, UserService>();
        services.AddValidatorsFromAssemblyContaining<IValidatorMarker>(ServiceLifetime.Singleton);
        return services;
    } 
}