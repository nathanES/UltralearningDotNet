using TaskManagement.Common.Interfaces.Commands;
using TaskManagement.Common.Middleware;
using TaskManagement.Common.ResultPattern;

namespace TaskManagement.Common.Commands;

public class ExistUserCommand(Guid id) : IRequest<Result<bool>>
{
    public Guid Id { get; } = id;
}