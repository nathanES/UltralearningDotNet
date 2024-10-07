using TaskManagement.Common.Middleware;
using TaskManagement.Tasks.Models;

namespace TaskManagement.Tasks.Commands.GetAllTasks;

public class GetAllTasksCommand : IRequest<Result<IEnumerable<Task>>>
{
    public string? Title { get; set; }

    public string? Description { get; set; }

    public DateTime? DeadLine { get; set; }
    
    public Priority? Priority { get; set; }

    public Status? Status { get; set; }

    public int Page { get; set; }

    public int PageSize { get; set; } 
}