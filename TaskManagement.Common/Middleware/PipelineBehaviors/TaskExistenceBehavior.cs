using Microsoft.Extensions.Logging;
using TaskManagement.Common.Commands;
using TaskManagement.Common.Interfaces;
using TaskManagement.Common.Interfaces.Commands;
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
        var (taskId, shouldExist) = GetTaskExistenceDetails(request);
        if (!taskId.HasValue)
        {
            logger.LogInformation("TaskId don't have value");
            return await next(); 
        }
        
        var isTaskExistResult =
            await mediator.SendAsync<ExistTaskCommand, Result<bool>>(new ExistTaskCommand(taskId.Value),
                cancellationToken); 
        
        if (isTaskExistResult.IsFailure)
        {
            logger.LogWarning("A technical error occurred while checking for task existence");
            return CreateFailureResult<TResponse>(isTaskExistResult.Errors);
        }

        if (shouldExist && !isTaskExistResult.Response)
        {
            logger.LogWarning("Task does not exist");
            return CreateFailureResult<TResponse>(new NotFoundError("Task", taskId.ToString()));

        }

        if (!shouldExist && isTaskExistResult.Response)
        {
            logger.LogWarning("Task with the same ID already exists");
            return CreateFailureResult<TResponse>(new ConflictError("Task already exists", "Task", taskId.ToString()));
        }

        return await next(); 
    }
    private (Guid? taskId, bool shouldExist) GetTaskExistenceDetails(TRequest request)
    {
        return request switch
        {
            IShouldTaskExistCommand existCommand => (existCommand.TaskId, true),
            IShouldTaskExistTaskCommand existTaskCommand => (existTaskCommand.Id, true),
            IShouldTaskNotExistCommand notExistCommand => (notExistCommand.TaskId, false),
            IShouldTaskNotExistTaskCommand notExistTaskCommand => (notExistTaskCommand.Id, false),
            _ => (null, false)
        };
    }
    
    private TResponse CreateFailureResult<T>(Error error)
    {
        var resultType = typeof(Result<>).MakeGenericType(typeof(TResponse));
        var failureResult = Activator.CreateInstance(resultType, error);
        return (TResponse)failureResult!;
    }
    private TResponse CreateFailureResult<T>(List<Error> errors)
    {
        var resultType = typeof(Result<>).MakeGenericType(typeof(TResponse));
        var failureResult = Activator.CreateInstance(resultType, errors);
        return (TResponse)failureResult!;
    }

}