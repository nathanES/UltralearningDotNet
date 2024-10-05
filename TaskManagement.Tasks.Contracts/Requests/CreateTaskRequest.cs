using TaskManagement.Common.Mediator;

namespace TaskManagement.Tasks.Contracts.Requests;

public class CreateTaskRequest 
{
    public required string Title { get; init; }
    public string Description { get; init; } = string.Empty;
    public DateTime? DeadLine { get; init; }
    public Priority? Priority { get; init; }
    public Status? Status { get; init; }
}