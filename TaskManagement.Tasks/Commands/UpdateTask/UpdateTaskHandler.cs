using TaskManagement.Common.Middleware;
using TaskManagement.Tasks.Interfaces;

namespace TaskManagement.Tasks.Commands.UpdateTask;

internal class UpdateTaskHandler(ITaskService taskService) : IRequestHandler<UpdateTaskCommand, Result<Task>>
{
    public async Task<Result<Task>> HandleAsync(UpdateTaskCommand request, CancellationToken token = default)
    {
        var task = new Task(request.Id,
            request.Title,
            request.Description,
            request.Deadline,
            request.Priority,
            request.Status,
            request.UserId);
        return await taskService.UpdateAsync(task, token);
    }
}