using TaskManagement.Tasks.Models;

namespace TaskManagement.Tasks.Interfaces;

public interface ITaskRepository
{
    Task<bool> CreateAsync(TaskManagement.Common.Models.Task task, CancellationToken token = default);
    Task<TaskManagement.Common.Models.Task?> GetByIdAsync(Guid id, CancellationToken token = default);
    Task<IEnumerable<TaskManagement.Common.Models.Task>> GetAllAsync(GetAllTasksOptions options, CancellationToken token = default);
    Task<bool> UpdateAsync(TaskManagement.Common.Models.Task task, CancellationToken token = default);
    Task<bool> DeleteByIdAsync(Guid id, CancellationToken token = default);
    Task<bool> ExistsByIdAsync(Guid id, CancellationToken token = default);

}