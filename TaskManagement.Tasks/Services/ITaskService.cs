using TaskManagement.Tasks.Models;
using Task = TaskManagement.Common.Models.Task;

namespace TaskManagement.Tasks.Services;

public interface ITaskService
{
    Task<bool> CreateAsync(Task task, CancellationToken token = default);
    Task<Task?> GetByIdAsync(Guid id, CancellationToken token = default);
    Task<IEnumerable<Task>> GetAllAsync(GetAllTasksOptions options, CancellationToken token = default);
    Task<Task?> UpdateAsync(Task task, CancellationToken token = default);
    Task<bool> DeleteByIdAsync(Guid id, CancellationToken token = default);
}