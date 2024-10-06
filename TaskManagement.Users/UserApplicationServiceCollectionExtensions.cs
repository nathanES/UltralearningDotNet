using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using TaskManagement.Common;
using TaskManagement.Common.Mediator;
using TaskManagement.Common.Models;
using TaskManagement.Users.Commands.CreateUser;
using TaskManagement.Users.Commands.DeleteUser;
using TaskManagement.Users.Commands.GetAllUsers;
using TaskManagement.Users.Commands.GetUser;
using TaskManagement.Users.Commands.UpdateUser;
using TaskManagement.Users.Interfaces;
using TaskManagement.Users.Services;
using TaskManagement.Users.Validators;

namespace TaskManagement.Users;

public static class UserApplicationServiceCollectionExtensions
{
    public static IServiceCollection AddUserApplication(this IServiceCollection services)
    {
        services.TryAddCommonServices();
        services.AddScoped<IUserService, UserService>();
        
        services.AddScoped<IRequestHandler<CreateUserCommand, bool>, CreateUserHandler>();
        services.AddScoped<IRequestHandler<DeleteUserCommand, bool>, DeleteUserHandler>();
        services.AddScoped<IRequestHandler<GetAllUsersCommand, IEnumerable<User>>, GetAllUsersHandler>();
        services.AddScoped<IRequestHandler<GetUserCommand, User?>, GetUserHandler>();
        services.AddScoped<IRequestHandler<UpdateUserCommand, User?>, UpdateUserHandler>();

        services.AddValidatorsFromAssemblyContaining<IValidatorMarker>(ServiceLifetime.Singleton);
        return services;
    } 
}