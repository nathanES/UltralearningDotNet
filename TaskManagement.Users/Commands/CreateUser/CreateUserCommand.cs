using TaskManagement.Common.Commands;
using TaskManagement.Common.Interfaces;
using TaskManagement.Common.Interfaces.Commands;
using TaskManagement.Common.Middleware;
using TaskManagement.Common.Models;
using TaskManagement.Common.ResultPattern;

namespace TaskManagement.Users.Commands.CreateUser;

public class CreateUserCommand(Guid id, string username, string email) : IRequest<Result<User>>, IUserCommand, ICreateCommand
{
    public Guid Id { get; } = id;
    public string Username { get; } = username;
    public string Email { get; } = email;
}