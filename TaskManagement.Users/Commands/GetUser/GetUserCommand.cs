using TaskManagement.Common.Commands;
using TaskManagement.Common.Interfaces;
using TaskManagement.Common.Interfaces.Commands;
using TaskManagement.Common.Middleware;
using TaskManagement.Common.Models;
using TaskManagement.Common.ResultPattern;
using TaskManagement.Users.Models;

namespace TaskManagement.Users.Commands.GetUser;

public class GetUserCommand(Guid id) : IRequest<Result<User>>, IUserCommand
{
    public Guid Id { get; } = id;
}