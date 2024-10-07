using FluentValidation;
using Microsoft.Extensions.Logging;
using TaskManagement.Common.ResultPattern;
using TaskManagement.Common.ResultPattern.Errors;

namespace TaskManagement.Common.Middleware.PipelineBehaviors;

internal class ValidationBehavior<TRequest, TResponse>(IEnumerable<IValidator<TRequest>> validators, ILogger<ValidationBehavior<TRequest, TResponse>> logger) : IPipelineBehavior<TRequest, TResponse> 
    where TRequest : IRequest<TResponse>
{
    public async Task<TResponse> HandleAsync(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken token = default)
    {
        if (!validators.Any())
        {
            logger.LogInformation("No validators found for the request");
            return await next();
        }

        var context = new ValidationContext<TRequest>(request);
        var validationResults = await Task.WhenAll(validators.Select(v => v.ValidateAsync(context, token)));
        var failures = validationResults.SelectMany(r => r.Errors).Where(f => f != null).ToList();
        if (failures.Count == 0)
        {
            logger.LogInformation("Request passed validation");
            return await next();
        }

        var errorMessage = string.Join("; ", failures.Select(f => f.ErrorMessage));
        logger.LogWarning("Validation failed: {ErrorMessage}", errorMessage);
        var validationError = new ValidationError(errorMessage, typeof(TRequest).Name);
                
        // If you're using the Result pattern, return a failure result
        var resultType = typeof(Result<>).MakeGenericType(typeof(TResponse));
        var failureResult = Activator.CreateInstance(resultType, validationError);
        return (TResponse)failureResult!; 
    }
}