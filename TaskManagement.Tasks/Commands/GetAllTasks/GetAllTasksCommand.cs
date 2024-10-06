using TaskManagement.Common.Mediator;
using TaskManagement.Common.Models;
using Task = TaskManagement.Common.Models.Task;

namespace TaskManagement.Tasks.Commands.GetAllTasks;

public class GetAllTasksCommand : IRequest<IEnumerable<Task>>
{
    public string? Title { get; set; }

    public string? Description { get; set; }

    public DateTime? DeadLine { get; set; }
    
    public Priority? Priority { get; set; }

    public Status? Status { get; set; }

    public int Page { get; set; }

    public int PageSize { get; set; } 
}