using TaskManagement.Common.Interfaces;
using TaskManagement.Common.Interfaces.Commands;
using TaskManagement.Common.Middleware;
using TaskManagement.Common.ResultPattern;

namespace TaskManagement.Common.Commands;

public class ExistTaskCommand(Guid id):IRequest<Result<bool>>, ITaskCommand
{
    public Guid Id { get; } = id;
}