using TaskManagement.Common.Middleware;
using TaskManagement.Common.ResultPattern;

namespace TaskManagement.Users.Commands.DeleteUser;

public class DeleteUserCommand(Guid id) : IRequest<Result<None>>
{
    public Guid Id { get; } = id;
}