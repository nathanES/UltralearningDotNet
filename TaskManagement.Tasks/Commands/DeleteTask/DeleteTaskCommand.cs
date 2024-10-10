using TaskManagement.Common.Commands;
using TaskManagement.Common.Interfaces;
using TaskManagement.Common.Interfaces.Commands;
using TaskManagement.Common.Middleware;
using TaskManagement.Tasks.Interfaces;

namespace TaskManagement.Tasks.Commands.DeleteTask;

public class DeleteTaskCommand(Guid id) 
    : IRequest<Result<None>>, IShouldTaskExistTaskCommand
{
    public Guid Id { get; } = id;
}