using TaskManagement.Common.Commands;
using TaskManagement.Common.Interfaces;
using TaskManagement.Common.Interfaces.Commands;
using TaskManagement.Common.Middleware;
using TaskManagement.Common.ResultPattern;

namespace TaskManagement.Users.Commands.DeleteUser;

public class DeleteUserCommand(Guid id) 
    : IRequest<Result<None>>, IShouldUserExistUserCommand
{
    public Guid Id { get; } = id;
}