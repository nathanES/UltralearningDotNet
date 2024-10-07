using TaskManagement.Common.Middleware;
using TaskManagement.Tasks.Interfaces;

namespace TaskManagement.Tasks.Commands.GetTask;

internal class GetTaskHandler(ITaskService taskService) : IRequestHandler<GetTaskCommand, Result<Task>>
{
    public async Task<Result<Task>> HandleAsync(GetTaskCommand request, CancellationToken token = default)
    {
        return await taskService.GetByIdAsync(request.Id, token);
    }
}