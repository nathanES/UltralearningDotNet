using System.Reflection;
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
            return CreateFailureResult(isUserExistResult.Errors);
        }

        // Handle user existence logic
        if (shouldExist && !isUserExistResult.Response)
        {
            logger.LogWarning("User with ID {UserId} does not exist", userId);
            return CreateFailureResult(new ConflictError("User doesn't exist", "User", userId.ToString()));
        }

        if (!shouldExist && isUserExistResult.Response)
        {
            logger.LogWarning("User with ID {UserId} already exists", userId);
            return CreateFailureResult(new ConflictError("User already exists", "User", userId.ToString()));
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
    private TResponse CreateFailureResult(Error error)
    {
        // Directly create a failure result of the expected TResponse type
        var resultType = typeof(TResponse);
        var bindingFlags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic;
        var constructor = resultType.GetConstructor(bindingFlags,new[] { typeof(Error) });
        
        if (constructor != null)
        {
            return (TResponse)constructor.Invoke(new object[] { error });
        }

        throw new InvalidOperationException($"Unable to create failure result of type {resultType}");
    }

    private TResponse CreateFailureResult(List<Error> errors)
    {
        // Directly create a failure result of the expected TResponse type using a List of Errors
        var resultType = typeof(TResponse);
        var bindingFlags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic;

        var constructor = resultType.GetConstructor(bindingFlags, new[] { typeof(List<Error>) });
        
        if (constructor != null)
        {
            return (TResponse)constructor.Invoke(new object[] { errors });
        }

        throw new InvalidOperationException($"Unable to create failure result of type {resultType}");
    }
}