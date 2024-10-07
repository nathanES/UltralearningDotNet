using Microsoft.Extensions.Logging;
using TaskManagement.Common.Commands;
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
        // If the request has a UserId property, validate that the user exists.
        var userIdProperty = request.GetType().GetProperty("UserId");
        if (userIdProperty == null)
        {
            return await next();
        }
        
        var userId = (Guid?)userIdProperty.GetValue(request);

        if (userId.HasValue)
        {
            var isUserExistResult =
                await mediator.SendAsync<ExistUserCommand, Result<bool>>(new ExistUserCommand(userId.Value),
                    cancellationToken);

            if (isUserExistResult.IsFailure)
            {
                logger.LogWarning("A technical error occurred while checking for user existence");
                var resultType = typeof(Result<>).MakeGenericType(typeof(TResponse));
                var failureResult = Activator.CreateInstance(resultType, isUserExistResult.Errors);
                return (TResponse)failureResult!; // Return failure result
            }

            if (!isUserExistResult.Response)
            {
                logger.LogWarning("User doesn't exist");
                var conflictError = new ConflictError("User doesn't exist", "User", userId.ToString());
                var resultType = typeof(Result<>).MakeGenericType(typeof(TResponse));
                var failureResult = Activator.CreateInstance(resultType, conflictError);
                return (TResponse)failureResult!;
            }
        }


        // Proceed to the next behavior or handler
        return await next();
    }
}