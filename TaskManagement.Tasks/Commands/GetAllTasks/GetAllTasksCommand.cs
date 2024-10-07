using TaskManagement.Common.Middleware;
using TaskManagement.Common.Models;

namespace TaskManagement.Tasks.Commands.GetAllTasks;

public class GetAllTasksCommand(string? title, string? description, DateTime? deadline, Priority? priority, Status? status, int page, int pageSize) : IRequest<Result<IEnumerable<Task>>>
{
    public string? Title { get; } = title;

    public string? Description { get; } = description;

    public DateTime? Deadline { get; } = deadline;

    public Priority? Priority { get; } = priority;

    public Status? Status { get; } = status;

    public int Page { get; } = page;

    public int PageSize { get; } = pageSize;
}