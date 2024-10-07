using TaskManagement.Common.Middleware;
using TaskManagement.Tasks.Models;

namespace TaskManagement.Tasks.Commands.UpdateTask;

public class UpdateTaskCommand : IRequest<Result<Task>>
{
    public required Guid Id { get; init; } 
    public required string Title { get; init; }
    public string Description { get; init; }
    public DateTime? DeadLine { get; init; }
    public Priority? Priority { get; init; }
    public Status? Status { get; init; }
}