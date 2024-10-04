using TaskManagement.Tasks.Models;

namespace TaskManagement.Tasks.Repositories;

public interface ITaskRepository
{
    Task<bool> CreateAsync(Models.Task task, CancellationToken token = default);
    Task<Models.Task?> GetByIdAsync(Guid id, CancellationToken token = default);
    Task<IEnumerable<Models.Task>> GetAllAsync(GetAllTasksOptions options, CancellationToken token = default);
    Task<bool> UpdateAsync(Models.Task task, CancellationToken token = default);
    Task<bool> DeleteByIdAsync(Guid id, CancellationToken token = default);
    Task<bool> ExistsByIdAsync(Guid id, CancellationToken token = default);

}