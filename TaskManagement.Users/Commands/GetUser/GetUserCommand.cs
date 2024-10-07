using TaskManagement.Common.Middleware;
using TaskManagement.Common.ResultPattern;
using TaskManagement.Users.Models;

namespace TaskManagement.Users.Commands.GetUser;

public class GetUserCommand(Guid id) : IRequest<Result<User>>
{
    public Guid Id { get; } = id;
}