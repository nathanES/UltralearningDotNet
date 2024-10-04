using TaskManagement.Tasks.Models;

namespace TaskManagement.Tasks.Services;

public interface ITaskService
{
    Task<bool> CreateAsync(Models.Task task, CancellationToken token = default);
    Task<Models.Task?> GetByIdAsync(Guid id, CancellationToken token = default);
    Task<IEnumerable<Models.Task>> GetAllAsync(GetAllTasksOptions options, CancellationToken token = default);
    Task<Models.Task?> UpdateAsync(Models.Task task, CancellationToken token = default);
    Task<bool> DeleteByIdAsync(Guid id, CancellationToken token = default);
}