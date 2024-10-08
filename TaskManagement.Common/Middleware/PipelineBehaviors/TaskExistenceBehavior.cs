using Microsoft.Extensions.Logging;
using TaskManagement.Common.Commands;
using TaskManagement.Common.ResultPattern;
using TaskManagement.Common.ResultPattern.Errors;

namespace TaskManagement.Common.Middleware.PipelineBehaviors;

public class TaskExistenceBehavior<TRequest, TResponse>(
    IMediator mediator,
    ILogger<TaskExistenceBehavior<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
{
    public async Task<TResponse> HandleAsync(TRequest request, RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (request is not ITaskCommand taskCommand)
        {
            return await next();
        }

        var isCreateCommand = request.GetType().Name.Contains("CreateTaskCommand");
        var isUpdateCommand = request.GetType().Name.Contains("UpdateTaskCommand");
        
        var taskId = taskCommand.Id;
        var isTaskExistResult =
            await mediator.SendAsync<ExistTaskCommand, Result<bool>>(new ExistTaskCommand(taskId),
                cancellationToken);
        
        if (isTaskExistResult.IsFailure)
        {
            logger.LogWarning("A technical error occurred while checking for task existence");
            var failureResult = Activator.CreateInstance(typeof(Result<>).MakeGenericType(typeof(TResponse)),
                isTaskExistResult.Errors);
            return (TResponse)failureResult!;
        }

        if (!isTaskExistResult.Response && isUpdateCommand)
        {
            logger.LogWarning("Task does not exist");
            var notFoundError = new NotFoundError("Task", taskId.ToString());
            var failureResult =
                Activator.CreateInstance(typeof(Result<>).MakeGenericType(typeof(TResponse)), notFoundError);
            return (TResponse)failureResult!;
        }

        if (isTaskExistResult.Response && isCreateCommand)
        {
            logger.LogWarning("Task with the same ID already exists");
            var conflictError = new ConflictError("Task already exists", "Task", taskId.ToString());
            var failureResult =
                Activator.CreateInstance(typeof(Result<>).MakeGenericType(typeof(TResponse)), conflictError);
            return (TResponse)failureResult!;
        }

        return await next();
    }
}