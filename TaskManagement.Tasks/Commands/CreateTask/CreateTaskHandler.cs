using TaskManagement.Common.Middleware;
using TaskManagement.Tasks.Interfaces;

namespace TaskManagement.Tasks.Commands.CreateTask;

internal class CreateTaskHandler(ITaskService taskService) : IRequestHandler<CreateTaskCommand, Result<Task>>
{
    public async Task<Result<Task>> HandleAsync(CreateTaskCommand request, CancellationToken token = default)
    {
        var task = new Task(
            Guid.NewGuid(),
            request.Title,
            request.Description,
            request.DeadLine,
            request.Priority,
            request.Status
        );

        return await taskService.CreateAsync(task, token);
    }
}