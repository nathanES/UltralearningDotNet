using TaskManagement.Common.Mediator;
using Task = TaskManagement.Common.Models.Task;

namespace TaskManagement.Tasks.Commands.GetTask;

public class GetTaskCommand(Guid id) : IRequest<Task?>
{
    public Guid Id { get; } = id;
}