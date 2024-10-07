using TaskManagement.Common.Middleware;
using TaskManagement.Tasks.Models;

namespace TaskManagement.Tasks.Commands.CreateTask;

public class CreateTaskCommand(Guid id, string title, string description, DateTime? deadline, Priority? priority, Status? status)
    : IRequest<Result<Task>>
{
    public Guid Id { get; } = id;
    public string Title { get; } = title;
    public string Description { get; } = description;
    public DateTime? DeadLine { get; } = deadline;
    public Priority? Priority { get; } = priority;
    public Status? Status { get; } = status;
}