using TaskManagement.Common.Mediator;
using TaskManagement.Tasks.Interfaces;

namespace TaskManagement.Tasks.Commands.DeleteTask;

internal class DeleteTaskHandler(ITaskService taskService) : IRequestHandler<DeleteTaskCommand, bool>
{
    public async Task<bool> HandleAsync(DeleteTaskCommand request, CancellationToken token = default)
    {
        return await taskService.DeleteByIdAsync(request.Id, token);
    }
}