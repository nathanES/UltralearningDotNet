using TaskManagement.Common.Commands;
using TaskManagement.Common.Interfaces;
using TaskManagement.Common.Interfaces.Commands;
using TaskManagement.Common.Middleware;

namespace TaskManagement.Tasks.Commands.GetTask;

public class GetTaskCommand(Guid id) 
    : IRequest<Result<Task>>
{
    public Guid Id { get; } = id;
}