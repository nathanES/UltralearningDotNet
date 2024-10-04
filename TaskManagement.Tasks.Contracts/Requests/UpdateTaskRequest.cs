namespace TaskManagement.Tasks.Contracts.Requests;

public class UpdateTaskRequest
{
    public required string Title { get; init; }
    public string? Description { get; init; }
    public DateTime? DeadLine { get; init; }
    public Priority? Priority { get; init; }
    public Status? Status { get; init; }
}