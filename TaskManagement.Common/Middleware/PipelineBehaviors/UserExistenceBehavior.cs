using Microsoft.Extensions.Logging;
using TaskManagement.Common.Commands;
using TaskManagement.Common.Interfaces;
using TaskManagement.Common.Interfaces.Commands;
using TaskManagement.Common.ResultPattern;
using TaskManagement.Common.ResultPattern.Errors;

namespace TaskManagement.Common.Middleware.PipelineBehaviors;

public class UserExistenceBehavior<TRequest, TResponse>(
    IMediator mediator,
    ILogger<UserExistenceBehavior<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
{
    public async Task<TResponse> HandleAsync(TRequest request, RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (request is not IUserCommand && request is not IShouldCheckUserExistenceCommand)
        {
            return await next();
        }

        // If the request has a UserId property, validate that the user exists.
        var userId = request is IUserCommand userCommand
            ? userCommand.Id
            : ((IShouldCheckUserExistenceCommand)request).UserId;
        
        if (!userId.HasValue)
        {
            logger.LogInformation("UserId don't have value");
            return await next();
        }

        var isUserExistResult =
            await mediator.SendAsync<ExistUserCommand, Result<bool>>(new ExistUserCommand(userId.Value),
                cancellationToken);

        if (isUserExistResult.IsFailure)
        {
            logger.LogWarning("A technical error occurred while checking for user existence");
            return CreateFailureResult<TResponse>(isUserExistResult.Errors);
        }

        var isCreateCommand = request is ICreateCommand;
        var isUpdateCommand = request is IUpdateCommand;
        
        if (!isUserExistResult.Response && isUpdateCommand)
        {
            logger.LogWarning("User doesn't exist");
            return CreateFailureResult<TResponse>(new ConflictError("User doesn't exist", "User", userId.ToString()));
        }
        
        if (isUserExistResult.Response && isCreateCommand)
        {
            logger.LogWarning("User with the same ID already exists");
            return CreateFailureResult<TResponse>(new ConflictError("User already exist", "User", userId.ToString()));
        }
        
        // Proceed to the next behavior or handler
        return await next();
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