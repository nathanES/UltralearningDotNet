namespace TaskManagement.Tasks.Contracts.Responses;

public class TaskResponse(
    Guid id,
    string title,
    string? description,
    DateTime? deadline,
    Priority? priority,
    Status? status,
    Guid? userId)
{
    public Guid Id { get; } = id;
    public string Title { get; } = title;
    public string? Description { get; } = description;
    public DateTime? Deadline { get; } = deadline;
    public Priority? Priority { get; } = priority;
    public Status? Status { get; } = status;
    public Guid? UserId { get; } = userId;
}