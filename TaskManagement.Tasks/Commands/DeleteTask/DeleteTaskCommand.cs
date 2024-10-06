using TaskManagement.Common.Mediator;
using TaskManagement.Tasks.Interfaces;

namespace TaskManagement.Tasks.Commands.DeleteTask;

public class DeleteTaskCommand(Guid id) : IRequest<bool>
{
    public Guid Id { get; } = id;
}