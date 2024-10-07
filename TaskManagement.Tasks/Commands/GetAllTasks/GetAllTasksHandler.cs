using TaskManagement.Common.Middleware;
using TaskManagement.Tasks.Interfaces;
using TaskManagement.Tasks.Models;

namespace TaskManagement.Tasks.Commands.GetAllTasks;

internal class GetAllTasksHandler(ITaskService taskService) : IRequestHandler<GetAllTasksCommand, Result<IEnumerable<Task>>>
{
    public async Task<Result<IEnumerable<Task>>> HandleAsync(GetAllTasksCommand request, CancellationToken token = default)
    {
        var getAllTasksOptions = new GetAllTasksOptions
        {
            Title = request.Title,
            Description = request.Description,
            DeadLine = request.DeadLine,
            Priority = request.Priority,
            Status = request.Status,
            Page = request.Page,
            PageSize = request.PageSize
        };
        return await taskService.GetAllAsync(getAllTasksOptions, token);
    }
}