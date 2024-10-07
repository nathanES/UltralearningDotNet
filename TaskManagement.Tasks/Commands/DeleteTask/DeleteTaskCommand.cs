using TaskManagement.Common.Middleware;
using TaskManagement.Tasks.Interfaces;

namespace TaskManagement.Tasks.Commands.DeleteTask;

public class DeleteTaskCommand(Guid id) : IRequest<Result<None>>
{
    public Guid Id { get; } = id;
}