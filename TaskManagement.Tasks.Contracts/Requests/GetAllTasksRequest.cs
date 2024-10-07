namespace TaskManagement.Tasks.Contracts.Requests;

public class GetAllTasksRequest : PagedRequest
{
    public string? Title { get; set; }
    public string? Description { get; set; }
    public DateTime? Deadline { get; set; }
    public Priority? Priority { get; set; }
    public Status? Status { get; set; } 
}