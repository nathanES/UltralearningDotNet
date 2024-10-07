using Ardalis.GuardClauses;

namespace TaskManagement.Tasks.Models;

public class Task(Guid id, string title, string description, DateTime? deadLine, Priority? priority, Status? status)
{
    public Guid Id { get; init; } = Guard.Against.Default(id);
    public string Title { get; private set; } = Guard.Against.NullOrWhiteSpace(title);
    public string Description { get; private set; } = Guard.Against.Null(description);
    public DateTime? DeadLine { get; private set; } = deadLine;
    public Priority? Priority { get; private set; } = priority;
    public Status? Status { get; private set; } = status;

    public void UpdateTitle(string title) => Title = Guard.Against.NullOrWhiteSpace(title);
    public void UpdateDescription(string description) => Description = Guard.Against.Null(description);
    public void UpdateDeadLine(DateTime? deadLine) => DeadLine = deadLine;
    public void UpdateStatus(Status? status) => Status = status;
    public void UpdatePriority(Priority? priority) => Priority = priority;
}