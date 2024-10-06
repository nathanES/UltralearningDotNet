using System.Security.AccessControl;
using TaskManagement.Common.Mediator;
using TaskManagement.Common.Models;
using Task = TaskManagement.Common.Models.Task;

namespace TaskManagement.Tasks.Commands.UpdateTask;

public class UpdateTaskCommand : IRequest<Task?>
{
    public required Guid Id { get; init; } 
    public required string Title { get; init; }
    public string Description { get; init; }
    public DateTime? DeadLine { get; init; }
    public Priority? Priority { get; init; }
    public Status? Status { get; init; }
}