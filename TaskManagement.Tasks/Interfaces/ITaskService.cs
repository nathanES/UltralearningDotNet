using TaskManagement.Tasks.Models;

namespace TaskManagement.Tasks.Interfaces;

public interface ITaskService
{
    Task<Result<Task>> CreateAsync(Task task, CancellationToken token = default);
    Task<Result<Task>> GetByIdAsync(Guid id, CancellationToken token = default);
    Task<Result<IEnumerable<Task>>> GetAllAsync(GetAllTasksOptions options, CancellationToken token = default);
    Task<Result<Task>> UpdateAsync(Task task, CancellationToken token = default);
    Task<Result<None>> DeleteByIdAsync(Guid id, CancellationToken token = default);
    Task<Result<bool>> ExistAsync(Guid id, CancellationToken token = default);
}