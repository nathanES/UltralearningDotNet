using TaskManagement.Common.Commands;
using TaskManagement.Common.Middleware;
using TaskManagement.Common.ResultPattern;
using TaskManagement.Users.Interfaces;

namespace TaskManagement.Users.Integrations.Commands.ExistUser;

internal class ExistUserHandler(IUserService userService) : IRequestHandler<ExistUserCommand, Result<bool>>
{
    public async Task<Result<bool>> HandleAsync(ExistUserCommand request, CancellationToken token = default)
    {
        return await userService.ExistAsync(request.Id, token);
    }
}