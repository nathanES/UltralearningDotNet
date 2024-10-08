using TaskManagement.Common.Commands;
using TaskManagement.Common.Middleware;
using TaskManagement.Tasks.Interfaces;

namespace TaskManagement.Tasks.Integrations.Commands.ExistTask;

internal class ExistTaskHandler(ITaskService taskService) : IRequestHandler<ExistTaskCommand, Result<bool>>
{
    public async Task<Result<bool>> HandleAsync(ExistTaskCommand request, CancellationToken token = default)
    {
        return await taskService.ExistAsync(request.Id, token);
    }
}