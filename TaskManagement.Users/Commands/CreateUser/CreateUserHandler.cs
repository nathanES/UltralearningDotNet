using TaskManagement.Common.Middleware;
using TaskManagement.Common.ResultPattern;
using TaskManagement.Users.Interfaces;
using TaskManagement.Users.Models;

namespace TaskManagement.Users.Commands.CreateUser;

internal class CreateUserHandler(IUserService userService) : IRequestHandler<CreateUserCommand, Result<User>>
{
    public async Task<Result<User>> HandleAsync(CreateUserCommand request, CancellationToken token = default)
    {
        var user = new User(request.Id, request.Username, request.Email);
        return await userService.CreateAsync(user, token);
    }
}