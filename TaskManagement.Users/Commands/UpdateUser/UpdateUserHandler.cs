using TaskManagement.Common.Mediator;
using TaskManagement.Common.Models;
using TaskManagement.Users.Interfaces;
using TaskManagement.Users.Services;

namespace TaskManagement.Users.Commands.UpdateUser;

internal class UpdateUserHandler(IUserService userService) : IRequestHandler<UpdateUserCommand, User?>
{
    public async Task<User?> HandleAsync(UpdateUserCommand request, CancellationToken token = default)
    {
        var user = new User(request.Id, request.Username, request.Email);
        return await userService.UpdateAsync(user, token);
    }
}