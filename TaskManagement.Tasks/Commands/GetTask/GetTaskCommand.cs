using TaskManagement.Common.Middleware;

namespace TaskManagement.Tasks.Commands.GetTask;

public class GetTaskCommand(Guid id) : IRequest<Result<Task>>
{
    public Guid Id { get; } = id;
}