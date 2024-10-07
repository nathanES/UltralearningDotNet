using TaskManagement.Common.Middleware;
using TaskManagement.Tasks.Interfaces;

namespace TaskManagement.Tasks.Commands.DeleteTask;

internal class DeleteTaskHandler(ITaskService taskService) : IRequestHandler<DeleteTaskCommand, Result<None>>
{
    public async Task<Result<None>> HandleAsync(DeleteTaskCommand request, CancellationToken token = default)
    {
        return await taskService.DeleteByIdAsync(request.Id, token);
    }
}