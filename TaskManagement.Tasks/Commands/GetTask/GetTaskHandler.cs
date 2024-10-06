using TaskManagement.Common.Mediator;
using TaskManagement.Tasks.Interfaces;
using TaskManagement.Tasks.Services;
using Task = TaskManagement.Common.Models.Task;

namespace TaskManagement.Tasks.Commands.GetTask;

internal class GetTaskHandler(ITaskService taskService) : IRequestHandler<GetTaskCommand, Task?>
{
    public async Task<Task?> HandleAsync(GetTaskCommand request, CancellationToken token = default)
    {
        return await taskService.GetByIdAsync(request.Id, token);
    }
}