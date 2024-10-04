using FluentValidation;
using Microsoft.Extensions.Logging;
using TaskManagement.Tasks.Models;
using TaskManagement.Tasks.Repositories;
using Task = TaskManagement.Tasks.Models.Task;

namespace TaskManagement.Tasks.Services;

public class TaskService(ITaskRepository taskRepository, IValidator<Task> taskValidator, ILogger<TaskService> logger)
    : ITaskService
{
    public async Task<bool> CreateAsync(Task task, CancellationToken token = default)
    {
        await taskValidator.ValidateAndThrowAsync(task, token);
        return await taskRepository.CreateAsync(task, token);
    }

    public async Task<Task?> GetByIdAsync(Guid id, CancellationToken token = default)
    {
        return await taskRepository.GetByIdAsync(id, token);
    }

    public async Task<IEnumerable<Task>> GetAllAsync(GetAllTasksOptions options, CancellationToken token = default)
    {
        return await taskRepository.GetAllAsync(options, token);
    }

    public async Task<Task?> UpdateAsync(Task task, CancellationToken token = default)
    {
        await taskValidator.ValidateAndThrowAsync(task, token);

        var isTaskExisting = await taskRepository.ExistsByIdAsync(task.Id, token);
        if (!isTaskExisting)
            return null;
        await taskRepository.UpdateAsync(task, token);
        return task;
    }

    public async Task<bool> DeleteByIdAsync(Guid id, CancellationToken token = default)
    {
        return await taskRepository.DeleteByIdAsync(id, token);
    }
}