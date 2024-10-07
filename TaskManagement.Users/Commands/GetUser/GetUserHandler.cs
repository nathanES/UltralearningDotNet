using TaskManagement.Common.Middleware;
using TaskManagement.Common.Models;
using TaskManagement.Common.ResultPattern;
using TaskManagement.Users.Interfaces;
using TaskManagement.Users.Models;
using TaskManagement.Users.Services;

namespace TaskManagement.Users.Commands.GetUser;

internal class GetUserHandler(IUserService userService) : IRequestHandler<GetUserCommand, Result<User>>
{
    public async Task<Result<User>> HandleAsync(GetUserCommand request, CancellationToken token = default)
    {
        return await userService.GetByIdAsync(request.Id, token);
    }
}