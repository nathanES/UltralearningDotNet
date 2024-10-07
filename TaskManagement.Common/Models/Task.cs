using Ardalis.GuardClauses;

namespace TaskManagement.Common.Models;

public class Task(Guid id, string title, string description, DateTime? deadline, Priority? priority, Status? status, Guid? userId)
{
    public Guid Id { get; init; } = Guard.Against.Default(id);
    public string Title { get; private set; } = Guard.Against.NullOrWhiteSpace(title);
    public string Description { get; private set; } = Guard.Against.Null(description);
    public DateTime? Deadline { get; private set; } = deadline;
    public Priority? Priority { get; private set; } = priority;
    public Status? Status { get; private set; } = status;
    public Guid? UserId { get; private set; } = userId;

    public void UpdateTitle(string title) => Title = Guard.Against.NullOrWhiteSpace(title);
    public void UpdateDescription(string description) => Description = Guard.Against.Null(description);
    public void UpdateDeadline(DateTime? deadline) => Deadline = deadline;
    public void UpdateStatus(Status? status) => Status = status;
    public void UpdatePriority(Priority? priority) => Priority = priority;
    public void UpdateUserId(Guid? userId) => UserId = userId;
}