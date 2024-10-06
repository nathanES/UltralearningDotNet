using TaskManagement.Common.Mediator;
using TaskManagement.Common.Models;
using TaskManagement.Users.Interfaces;

namespace TaskManagement.Users.Commands.CreateUser;

internal class CreateUserHandler(IUserService userService) : IRequestHandler<CreateUserCommand, bool>
{
    public async Task<bool> HandleAsync(CreateUserCommand request, CancellationToken token = default)
    {
        var user = new User(request.Id, request.Username, request.Email);
        return await userService.CreateAsync(user, token);
    }
}