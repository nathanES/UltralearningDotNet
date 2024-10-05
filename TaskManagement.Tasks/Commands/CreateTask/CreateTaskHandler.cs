using TaskManagement.Common.Mediator;
using TaskManagement.Tasks.Interfaces;
using Task = TaskManagement.Common.Models.Task;

namespace TaskManagement.Tasks.Commands.CreateTask;

public class CreateTaskHandler(ITaskService taskService) : IRequestHandler<CreateTaskCommand, bool>
{
    public async Task<bool> Handle(CreateTaskCommand request, CancellationToken token = default)
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