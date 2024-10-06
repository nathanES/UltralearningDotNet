using TaskManagement.Common.Mediator;
using TaskManagement.Tasks.Interfaces;
using TaskManagement.Tasks.Services;
using Task = TaskManagement.Common.Models.Task;

namespace TaskManagement.Tasks.Commands.UpdateTask;

internal class UpdateTaskHandler(ITaskService taskService) : IRequestHandler<UpdateTaskCommand, Task?>
{
    public async Task<Task?> HandleAsync(UpdateTaskCommand request, CancellationToken token = default)
    {
        var task = new Task(request.Id,
            request.Title,
            request.Description,
            request.DeadLine,
            request.Priority,
            request.Status);
        return await taskService.UpdateAsync(task, token);
    }
}