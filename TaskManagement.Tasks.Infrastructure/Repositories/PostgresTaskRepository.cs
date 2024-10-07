using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TaskManagement.Common.ResultPattern.Errors;
using TaskManagement.Tasks.Infrastructure.Database;
using TaskManagement.Tasks.Interfaces;
using TaskManagement.Tasks.Models;

namespace TaskManagement.Tasks.Infrastructure.Repositories;

public class PostgresTaskRepository(ILogger<PostgresTaskRepository> logger, TasksContext context) : ITaskRepository
{
    public async Task<Result<Task>> CreateAsync(Task task, CancellationToken token = default)
    {
        logger.LogDebug($"{nameof(CreateAsync)} - Start");
        return await ResultExtensions.TryExecuteAsync(async () =>
        {
            await context.Tasks.AddAsync(task, token);
            await context.SaveChangesAsync(token);
            return Result<Task>.Success(task);
        }, ex =>
        {
            logger.LogError(ex, "An error occurred while creating task");

            return new List<Error>
            {
                new DatabaseError("Failed to create task", "Task",
                    JsonSerializer.Serialize(task, new JsonSerializerOptions { WriteIndented = true }),
                    details: ex.Message)
            };
        });
    }

    public async Task<Result<Task>> GetByIdAsync(Guid id, CancellationToken token = default)
    {
        logger.LogDebug($"{nameof(GetByIdAsync)} - Start");
        return await ResultExtensions.TryExecuteAsync(async () =>
        {
            var task = await context.Tasks.FindAsync(new object[] { id }, token);
            if (task is null)
            {
                logger.LogWarning("Task not found while trying to retrieving it");
                return Result<Task>.Failure(new NotFoundError("task", id.ToString()));
            }

            return Result<Task>.Success(task);
        }, ex =>
        {
            logger.LogError(ex, "An error occurred while retrieving task");

            return new List<Error>
            {
                new DatabaseError("Failed to retrieve task", "Task", id.ToString(), ex.Message)
            };
        });
    }

    public async Task<Result<IEnumerable<Task>>> GetAllAsync(GetAllTasksOptions options,
        CancellationToken token = default)
    {
        logger.LogDebug($"{nameof(GetAllAsync)} - Start");
        return await ResultExtensions.TryExecuteAsync(async () =>
        {
            var query = context.Tasks.AsNoTracking().AsQueryable();
            if (options.Description != null)
            {
                query = query.Where(t => t.Description!.Contains(options.Description));
            }

            if (options.Priority.HasValue)
            {
                query = query.Where(t => t.Priority == options.Priority);
            }

            if (options.DeadLine.HasValue)
            {
                query = query.Where(t => t.DeadLine == options.DeadLine);
            }

            if (options.Status.HasValue)
            {
                query = query.Where(t => t.Status == options.Status);
            }

            if (options.Title != null)
            {
                query = query.Where(t => t.Title == options.Title);
            }

            var tasks = await query
                .Skip((options.Page - 1) * options.PageSize)
                .Take(options.PageSize)
                .ToListAsync(token);
            return Result<IEnumerable<Task>>.Success(tasks);
        }, ex =>
        {
            logger.LogError(ex, "An error occurred while retrieving all tasks");

            return new List<Error>
            {
                new DatabaseError("Failed to retrieve all tasks", "Task",
                    JsonSerializer.Serialize(options, new JsonSerializerOptions { WriteIndented = true }),
                    details: ex.Message)
            };
        });
    }

    public async Task<Result<Task>> UpdateAsync(Task task, CancellationToken token = default)
    {
        logger.LogDebug($"{nameof(UpdateAsync)} - Start");
        return await ResultExtensions.TryExecuteAsync(async () =>
        {
            var taskToUpdate = await context.Tasks.FindAsync(new object[] { task.Id }, token);
            if (taskToUpdate is null)
            {
                logger.LogError("Task not found while trying to update it");
                return Result<Task>.Failure(new NotFoundError("task", task.Id.ToString()));
            }

            taskToUpdate.UpdateDescription(task.Description);
            taskToUpdate.UpdatePriority(task.Priority);
            taskToUpdate.UpdateStatus(task.Status);
            taskToUpdate.UpdateTitle(task.Title);
            taskToUpdate.UpdateDeadLine(task.DeadLine);

            await context.SaveChangesAsync(token);
            return Result<Task>.Success(taskToUpdate);
        }, ex =>
        {
            logger.LogError(ex, "An error occurred while updating task");

            return new List<Error>
            {
                new DatabaseError("Failed to update task", "Task", task.Id.ToString(), ex.Message)
            };
        });
    }

    public async Task<Result<None>> DeleteByIdAsync(Guid id, CancellationToken token = default)
    {
        logger.LogDebug($"{nameof(DeleteByIdAsync)} - Start");

        return await ResultExtensions.TryExecuteAsync(async () =>
            {
                var taskToDelete = await context.Tasks.FindAsync(new object[] { id }, token);

                if (taskToDelete is null)
                {
                    logger.LogError("Task not found while trying to delete it");
                    return Result<None>.Failure(new NotFoundError("task", id.ToString()));
                }

                context.Tasks.Remove(taskToDelete);
                await context.SaveChangesAsync(token);

                return Result<None>.Success(None.Value);
            },
            ex =>
            {
                logger.LogError(ex, "An error occurred while deleting task");

                return new List<Error>
                {
                    new DatabaseError("Failed to delete task", "Task", id.ToString(), ex.Message)
                };
            });
    }

    public async Task<Result<bool>> ExistsByIdAsync(Guid id, CancellationToken token = default)
    {
        logger.LogDebug($"{nameof(ExistsByIdAsync)} - Start");
        return await ResultExtensions.TryExecuteAsync(async () =>
        {
            var doesExist = await context.Tasks.AnyAsync(x => x.Id == id, token);
            logger.LogDebug($"{nameof(ExistsByIdAsync)} - Task existence: {doesExist}");
            return Result<bool>.Success(doesExist);
        }, ex =>
        {
            logger.LogError(ex, "An error occurred while checking if the task exists.");
            return new List<Error>()
            {
                new DatabaseError("Failed to check if task exists.", "Task", id.ToString(), ex.Message)
            };
        });
    }
    private async Task<Result<Task>> GetTaskByIdOrFail(Guid id, CancellationToken token = default)
    {
        return await ResultExtensions.TryExecuteAsync(async () =>
        {
            var task = await context.Tasks.FindAsync(new object[] { id }, token);
            if (task is null)
            {
                return Result<Task>.Failure(new NotFoundError("task", id.ToString()));
            }
            return Result<Task>.Success(task);
        }, ex =>
        {
            logger.LogError(ex, "An error occurred while retrieving the task");
            return new List<Error>
            {
                new DatabaseError("Failed to retrieve task", "Task", id.ToString(), ex.Message)
            };
        });
    }

    
}