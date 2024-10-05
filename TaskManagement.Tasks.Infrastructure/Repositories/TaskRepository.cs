using System.Reflection.Metadata;
using Microsoft.Extensions.Logging;
using TaskManagement.Tasks.Interfaces;
using TaskManagement.Tasks.Models;
using Task = TaskManagement.Common.Models.Task;

namespace TaskManagement.Tasks.Infrastructure.Repositories;

public class TaskRepository(ILogger<TaskRepository> logger) : ITaskRepository
{
    private readonly List<Task> _tasks = [];

    public Task<bool> CreateAsync(@Task task, CancellationToken token = default)
    {
        if (ExistsById(task.Id))
            return System.Threading.Tasks.Task.FromResult(false);;
        _tasks.Add(task);
        logger.LogDebug("Created");
        return System.Threading.Tasks.Task.FromResult(true);
    }

    public Task<Task?> GetByIdAsync(Guid id, CancellationToken token = default)
    {
        logger.LogDebug("Retrieved by Id");
        var task = _tasks.FirstOrDefault(x => x.Id == id); 
        return System.Threading.Tasks.Task.FromResult(task);
    }

    public Task<IEnumerable<Task>> GetAllAsync(GetAllTasksOptions options, CancellationToken token = default)
    {
        logger.LogDebug("Retrieved all");
        var result = _tasks
            .Skip((options.Page - 1) * options.PageSize)
            .Take(options.PageSize); 
        return System.Threading.Tasks.Task.FromResult<IEnumerable<Task>>(result);
    }

    public Task<bool> UpdateAsync(Task task, CancellationToken token = default)
    {
        var index = _tasks.FindIndex(x => x.Id == task.Id);
        if (index == -1)
            return System.Threading.Tasks.Task.FromResult(false);

        _tasks[index] = task;
        logger.LogDebug("Updated");
        return System.Threading.Tasks.Task.FromResult(true);
    }

    public Task<bool> DeleteByIdAsync(Guid id, CancellationToken token = default)
    {
        var task = _tasks.FirstOrDefault(x => x.Id == id);
        if (task == null)
            return System.Threading.Tasks.Task.FromResult(false);

        _tasks.Remove(task);
        logger.LogDebug("Deleted");
        return System.Threading.Tasks.Task.FromResult(true);
    }

    public Task<bool> ExistsByIdAsync(Guid id, CancellationToken token = default)
    {
        var exists = _tasks.Any(x => x.Id == id);
        logger.LogDebug(exists ? "Exists" : "Does not exist");
        return System.Threading.Tasks.Task.FromResult(exists);
    }
    private bool ExistsById(Guid id)
    {
        return _tasks.Any(x => x.Id == id);
    }
}