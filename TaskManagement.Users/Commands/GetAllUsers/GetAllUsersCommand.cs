using TaskManagement.Common.Middleware;
using TaskManagement.Common.ResultPattern;
using TaskManagement.Users.Models;

namespace TaskManagement.Users.Commands.GetAllUsers;

public class GetAllUsersCommand(string? username, string? email, int page, int pageSize) : IRequest<Result<IEnumerable<User>>>
{
    public string? Username { get; } = username;
    public string? Email { get; } = email;
    public int Page { get; } = page;
    public int PageSize { get; } = pageSize;
}