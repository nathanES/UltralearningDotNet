using TaskManagement.Common.Commands;
using TaskManagement.Common.Middleware;
using TaskManagement.Tasks.Interfaces;

namespace TaskManagement.Tasks.Commands.DeleteTask;

public class DeleteTaskCommand(Guid id) : IRequest<Result<None>>, ITaskCommand
{
    public Guid Id { get; } = id;
}