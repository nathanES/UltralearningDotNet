using TaskManagement.Common.Middleware;
using TaskManagement.Common.Models;
using TaskManagement.Common.ResultPattern;
using TaskManagement.Users.Interfaces;
using TaskManagement.Users.Models;
using TaskManagement.Users.Services;

namespace TaskManagement.Users.Commands.UpdateUser;

internal class UpdateUserHandler(IUserService userService) : IRequestHandler<UpdateUserCommand, Result<User>>
{
    public async Task<Result<User>> HandleAsync(UpdateUserCommand request, CancellationToken token = default)
    {
        var user = new User(request.Id, request.Username, request.Email);
        return await userService.UpdateAsync(user, token);
    }
}