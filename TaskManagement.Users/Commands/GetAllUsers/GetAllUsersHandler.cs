using TaskManagement.Common.Middleware;
using TaskManagement.Common.Models;
using TaskManagement.Common.ResultPattern;
using TaskManagement.Users.Interfaces;
using TaskManagement.Users.Models;
using TaskManagement.Users.Services;

namespace TaskManagement.Users.Commands.GetAllUsers;

internal class GetAllUsersHandler(IUserService userService) : IRequestHandler<GetAllUsersCommand, Result<IEnumerable<User>>>
{
    public async Task<Result<IEnumerable<User>>> HandleAsync(GetAllUsersCommand request, CancellationToken token = default)
    {
        var option = new GetAllUsersOptions
        {
            Username = request.Username,
            Email = request.Email,
            Page = request.Page,
            PageSize = request.PageSize
        };
        return await userService.GetAllAsync(option, token);
    }
}