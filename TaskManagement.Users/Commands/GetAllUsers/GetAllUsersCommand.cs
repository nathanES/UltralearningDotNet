using TaskManagement.Common.Mediator;
using TaskManagement.Common.Models;

namespace TaskManagement.Users.Commands.GetAllUsers;

public class GetAllUsersCommand(string? username, string? email, int page, int pageSize) : IRequest<IEnumerable<User>>
{
    public string? Username { get; } = username;
    public string? Email { get; } = email;
    public int Page { get; } = page;
    public int PageSize { get; } = pageSize;
}