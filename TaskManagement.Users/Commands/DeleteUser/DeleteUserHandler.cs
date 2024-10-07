using TaskManagement.Common.Middleware;
using TaskManagement.Common.ResultPattern;
using TaskManagement.Users.Interfaces;

namespace TaskManagement.Users.Commands.DeleteUser;

internal class DeleteUserHandler(IUserService userService) : IRequestHandler<DeleteUserCommand, Result<None>>
{
    public async Task<Result<None>> HandleAsync(DeleteUserCommand request, CancellationToken token = default)
    {
        return await userService.DeleteByIdAsync(request.Id, token);
    }
}