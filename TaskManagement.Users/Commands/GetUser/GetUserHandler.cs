using TaskManagement.Common.Mediator;
using TaskManagement.Common.Models;
using TaskManagement.Users.Interfaces;
using TaskManagement.Users.Services;

namespace TaskManagement.Users.Commands.GetUser;

internal class GetUserHandler(IUserService userService) : IRequestHandler<GetUserCommand, User?>
{
    public async Task<User?> HandleAsync(GetUserCommand request, CancellationToken token = default)
    {
        return await userService.GetByIdAsync(request.Id, token);
    }
}