using System.Collections;
using Microsoft.Extensions.Logging;
using TaskManagement.Common.Commands;
using TaskManagement.Common.Middleware;
using TaskManagement.Common.ResultPattern.Errors;
using TaskManagement.Tasks.Interfaces;
using TaskManagement.Tasks.Models;

namespace TaskManagement.Tasks.Services;

internal class TaskService(ITaskRepository taskRepository, ILogger<TaskService> logger, IMediator mediator)
    : ITaskService
{
    public async Task<Result<Task>> CreateAsync(Task task, CancellationToken token = default)
    {
        if (task == null)
        {
            logger.LogError("Null task passed to CreateAsync");
            return Result<Task>.Failure(new ValidationError("Task cannot be null", "Task"));
        }
        
        var isTaskExistResult = await taskRepository.ExistsByIdAsync(task.Id, token);
        if (isTaskExistResult.IsFailure)
        {
            logger.LogWarning("A technical error occurred while checking for task existence");
            return Result<Task>.Failure(isTaskExistResult.Errors);
        }

        if (isTaskExistResult.Response)
        {
            logger.LogWarning("Task with the same ID already exists");
            return Result<Task>.Failure(new ConflictError("Task already exists", "Task", task.Id.ToString()));
        }

        var taskCreateResult = await taskRepository.CreateAsync(task, token);
        if (taskCreateResult.IsFailure)
        {
            logger.LogWarning("A technical error occurred while updating task");
            return Result<Task>.Failure(taskCreateResult.Errors);
        }

        return Result<Task>.Success(taskCreateResult.Response);
    }

    public async Task<Result<Task>> GetByIdAsync(Guid id, CancellationToken token = default)
    {
        return await taskRepository.GetByIdAsync(id, token);
    }

    public async Task<Result<IEnumerable<Task>>> GetAllAsync(GetAllTasksOptions options,
        CancellationToken token = default)
    {
        if (options == null)
        {
            logger.LogError("Null options passed to GetAllAsync");
            return Result<IEnumerable<Task>>.Failure(new ValidationError("Options cannot be null", "Task"));
        } 
        return await taskRepository.GetAllAsync(options, token);
    }

    public async Task<Result<Task>> UpdateAsync(Task task, CancellationToken token = default)
    {
        if (task == null)
        {
            logger.LogError("Null task passed to UpdateAsync");
            return Result<Task>.Failure(new ValidationError("Task cannot be null", "Task"));
        }
        
        var isTaskExistResult = await taskRepository.ExistsByIdAsync(task.Id, token);
        if (isTaskExistResult.IsFailure)
        {
            logger.LogWarning("A technical error occurred while checking for task existence");
            return Result<Task>.Failure(isTaskExistResult.Errors);
        }

        if (!isTaskExistResult.Response)
        {
            logger.LogWarning("Task does not exist");
            return Result<Task>.Failure(new ConflictError("Task does not exist", "Task", task.Id.ToString()));
        }

        var updateTaskResult = await taskRepository.UpdateAsync(task, token);
        if (updateTaskResult.IsFailure)
        {
            logger.LogWarning("A technical error occurred while updating task");
            return Result<Task>.Failure(updateTaskResult.Errors);
        }

        return Result<Task>.Success(updateTaskResult.Response);
    }

    public async Task<Result<None>> DeleteByIdAsync(Guid id, CancellationToken token = default)
    {
        var isTaskExistResult = await taskRepository.ExistsByIdAsync(id, token);
        if (isTaskExistResult.IsFailure)
        {
            logger.LogWarning("A technical error occurred while checking for task existence");
            return Result<None>.Failure(isTaskExistResult.Errors);
        }

        if (!isTaskExistResult.Response)
        {
            logger.LogWarning("Task doesn't exist");
            return Result<None>.Failure(new NotFoundError("Task", id.ToString()));
        }
        return await taskRepository.DeleteByIdAsync(id, token);
    }

    public async Task<Result<bool>> ExistAsync(Guid id, CancellationToken token = default)
    {
        return await taskRepository.ExistsByIdAsync(id, token);
    }
}