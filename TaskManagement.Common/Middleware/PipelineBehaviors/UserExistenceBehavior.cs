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
        var (userId, shouldExist) = GetUserExistenceDetails(request);
        if (!userId.HasValue)
        {
            logger.LogInformation("UserId don't have value");
            return await next(); 
        }
        
        var isUserExistResult = await mediator.SendAsync<ExistUserCommand, Result<bool>>(new ExistUserCommand(userId.Value), cancellationToken);
            
        if (isUserExistResult.IsFailure)
        {
            logger.LogWarning("Error occurred while checking user existence: {Errors}", isUserExistResult.Errors);
            return CreateFailureResult<TResponse>(isUserExistResult.Errors);
        }

        // Handle user existence logic
        if (shouldExist && !isUserExistResult.Response)
        {
            logger.LogWarning("User with ID {UserId} does not exist", userId);
            return CreateFailureResult<TResponse>(new ConflictError("User doesn't exist", "User", userId.ToString()));
        }

        if (!shouldExist && isUserExistResult.Response)
        {
            logger.LogWarning("User with ID {UserId} already exists", userId);
            return CreateFailureResult<TResponse>(new ConflictError("User already exists", "User", userId.ToString()));
        }
        return await next();
    }
    private (Guid? userId, bool shouldExist) GetUserExistenceDetails(TRequest request)
    {
        return request switch
        {
            IShouldUserExistCommand existCommand => (existCommand.UserId, true),
            IShouldUserExistUserCommand existUserCommand => (existUserCommand.Id, true),
            IShouldUserNotExistCommand notExistCommand => (notExistCommand.UserId, false),
            IShouldUserNotExistUserCommand notExistUserCommand => (notExistUserCommand.Id, false),
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