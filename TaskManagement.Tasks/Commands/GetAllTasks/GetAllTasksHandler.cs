using TaskManagement.Common.Mediator;
using TaskManagement.Tasks.Interfaces;
using TaskManagement.Tasks.Models;
using TaskManagement.Tasks.Services;
using Task = TaskManagement.Common.Models.Task;

namespace TaskManagement.Tasks.Commands.GetAllTasks;

internal class GetAllTasksHandler(ITaskService taskService) : IRequestHandler<GetAllTasksCommand, IEnumerable<Task>>
{
    public async Task<IEnumerable<Task>> HandleAsync(GetAllTasksCommand request, CancellationToken token = default)
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