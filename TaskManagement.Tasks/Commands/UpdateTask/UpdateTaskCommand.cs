using TaskManagement.Common.Commands;
using TaskManagement.Common.Middleware;
using TaskManagement.Common.Models;

namespace TaskManagement.Tasks.Commands.UpdateTask;

public class UpdateTaskCommand(Guid id, string title, string description, DateTime? deadline, Priority? priority, Status? status, Guid? userId) 
    : IRequest<Result<Task>>, ITaskCommand
{
    public Guid Id { get; } = id;
    public string Title { get; } = title;
    public string Description { get; } = description;
    public DateTime? Deadline { get; } = deadline;
    public Priority? Priority { get; } = priority;
    public Status? Status { get; } = status;
    public Guid? UserId { get; } = userId;
}