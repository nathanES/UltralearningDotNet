using FluentValidation;
using Microsoft.Extensions.Logging;
using TaskManagement.Common.ResultPattern.Errors;
using TaskManagement.Tasks.Interfaces;
using TaskManagement.Tasks.Models;

namespace TaskManagement.Tasks.Services;

public class TaskService(ITaskRepository taskRepository, IValidator<Task> validator, ILogger<TaskService> logger)
    : ITaskService
{
    public async Task<Result<Task>> CreateAsync(Task task, CancellationToken token = default)
    {
        var validationResult = await ValidateTaskAsync(task, token);
        if (validationResult.IsFailure)
        {
            return Result<Task>.Failure(validationResult.Errors);
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
        return await taskRepository.CreateAsync(task, token);
    }

    public async Task<Result<Task>> GetByIdAsync(Guid id, CancellationToken token = default)
    {
        return await taskRepository.GetByIdAsync(id, token);
    }

    public async Task<Result<IEnumerable<Task>>> GetAllAsync(GetAllTasksOptions options, CancellationToken token = default)
    {
        return await taskRepository.GetAllAsync(options, token);
    }

    public async Task<Result<Task>> UpdateAsync(Task task, CancellationToken token = default)
    {
        var validationResult = await ValidateTaskAsync(task, token);
        if (validationResult.IsFailure)
        {
            return Result<Task>.Failure(validationResult.Errors);
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
        return await taskRepository.DeleteByIdAsync(id, token);
    }
    
    private async Task<Result<None>> ValidateTaskAsync(Task task, CancellationToken token)
    {
        try
        {
            await validator.ValidateAndThrowAsync(task, token);
            return Result<None>.Success(None.Value);
        }
        catch (ValidationException ex)
        {
            logger.LogWarning("Validation failed for task: {Errors}", ex.Message);
            return Result<None>.Failure(new ValidationError(ex.Message, "Task", task.Id.ToString()));
        }
    }

}