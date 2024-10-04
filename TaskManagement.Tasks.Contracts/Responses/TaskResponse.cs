namespace TaskManagement.Tasks.Contracts.Responses;

public class TaskResponse
{
    public required Guid Id { get; init; }
    public required string Title { get; init; }
    public string? Description { get; init; }
    public DateTime? DeadLine { get; init; }
    public Priority? Priority { get; init; }
    public Status? Status { get; init; } 
}