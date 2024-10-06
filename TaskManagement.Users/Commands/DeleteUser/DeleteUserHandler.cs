using TaskManagement.Common.Mediator;
using TaskManagement.Users.Interfaces;

namespace TaskManagement.Users.Commands.DeleteUser;

internal class DeleteUserHandler(IUserService userService) : IRequestHandler<DeleteUserCommand, bool>
{
    public async Task<bool> HandleAsync(DeleteUserCommand request, CancellationToken token = default)
    {
        return await userService.DeleteByIdAsync(request.Id, token);
    }
}