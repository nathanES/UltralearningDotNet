using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TaskManagement.Tasks.Infrastructure.Data;
using TaskManagement.Tasks.Models;
using TaskManagement.Tasks.Repositories;
using TaskManagement.Tasks.Repositories;
using Task = TaskManagement.Tasks.Models.Task;

namespace TaskManagement.Tasks.Infrastructure.Repositories;

public class PostgreTaskRepository(ILogger<PostgreTaskRepository> logger, TasksContext context) : ITaskRepository
{
    public async Task<bool> CreateAsync(Task task, CancellationToken token = default)
    {
        logger.LogDebug($"{nameof(CreateAsync)} - Start");

        if (await this.ExistsByIdAsync(task.Id, token))
            return false;
        await context.Tasks.AddAsync(task, token);
        await context.SaveChangesAsync(token);

        logger.LogDebug($"{nameof(CreateAsync)} - End");
        return true;
    }

    public async Task<Task?> GetByIdAsync(Guid id, CancellationToken token = default)
    {
        logger.LogDebug($"{nameof(GetByIdAsync)} - Start");
        var task = await context.Tasks.FindAsync(new object[] { id }, token);
        logger.LogDebug($"{nameof(GetByIdAsync)} - End");
        return task;
    }

    public async Task<IEnumerable<Task>> GetAllAsync(GetAllTasksOptions options, CancellationToken token = default)
    {
        logger.LogDebug($"{nameof(GetAllAsync)} - Start");
        var query = context.Tasks.AsQueryable();
        if (options.Description != null)
        {
            query.Where(t => t.Description!.Contains(options.Description));
        }

        if (options.Priority.HasValue)
        {
            query.Where(t=> t.Priority == options.Priority);
        }

        if (options.DeadLine.HasValue)
        {
            query.Where(t => t.DeadLine == options.DeadLine);
        }

        if (options.Status.HasValue)
        {
            query.Where(t => t.Status == options.Status);
        }

        if (options.Title != null)
        {
            query.Where(t => t.Title == options.Title);
        }
        
        var tasks = await query
            .Skip((options.Page - 1) * options.PageSize)
            .Take(options.PageSize)
            .ToListAsync(token);
        logger.LogDebug($"{nameof(GetAllAsync)} - End");
        return tasks;
    }

    public async Task<bool> UpdateAsync(Task task, CancellationToken token = default)
    {
        logger.LogDebug($"{nameof(UpdateAsync)} - Start");
        var taskToUpdate = await context.Tasks.FindAsync(task.Id, token);
        if (taskToUpdate is null)
            return false;

        taskToUpdate.UpdateDescription(task.Description);
        taskToUpdate.UpdatePriority(task.Priority);
        taskToUpdate.UpdateStatus(task.Status);
        taskToUpdate.UpdateTitle(task.Title);
        taskToUpdate.UpdateDeadLine(task.DeadLine);

        await context.SaveChangesAsync(token);
        logger.LogDebug($"{nameof(UpdateAsync)} - End");
        return true;
    }

    public async Task<bool> DeleteByIdAsync(Guid id, CancellationToken token = default)
    {
        logger.LogDebug($"{nameof(DeleteByIdAsync)} - Start");
        var taskToDelete = await context.Tasks.FindAsync(id, token);
        if (taskToDelete is null)
        {
            return false;
        }

        context.Tasks.Remove(taskToDelete);
        context.SaveChangesAsync(token);
        logger.LogDebug($"{nameof(DeleteByIdAsync)} - End");
        return true;
    }

    public async Task<bool> ExistsByIdAsync(Guid id, CancellationToken token = default)
    {
        logger.LogDebug($"{nameof(ExistsByIdAsync)} - Start");
        var doesExists = await context.Tasks.AnyAsync(x => x.Id == id, token);
        logger.LogDebug($"{nameof(ExistsByIdAsync)} - End");
        return doesExists;
    }
}