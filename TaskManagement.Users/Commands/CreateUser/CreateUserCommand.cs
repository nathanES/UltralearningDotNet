using TaskManagement.Common.Middleware;
using TaskManagement.Common.Models;
using TaskManagement.Common.ResultPattern;
using TaskManagement.Users.Models;

namespace TaskManagement.Users.Commands.CreateUser;

public class CreateUserCommand(Guid id, string username, string email) : IRequest<Result<User>>
{
    public Guid Id { get; } = id;
    public string Username { get; } = username;
    public string Email { get; } = email;
}