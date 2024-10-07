using TaskManagement.Common.ResultPattern.Errors;

namespace TaskManagement.Common.ResultPattern;

public static class ResultExtensions
{
    #region Bind
    public static Result<TResponse> Bind<TRequest, TResponse>(this Result<TRequest> result, Func<TRequest, Result<TResponse>> func)
    {
        if (result.IsFailure)
        {
            return Result<TResponse>.Failure(result.Error);
        }

        return func(result.Response);
    }
    public static async Task<Result<TResponse>> BindAsync<TRequest, TResponse>(this Task<Result<TRequest>> task, Func<TRequest, Result<TResponse>> func)
    {
        var result = await task;

        if (result.IsFailure)
        {
            return Result<TResponse>.Failure(result.Error);
        }

        return func(result.Response);
    }
 
    public static async Task<Result<TResponse>> BindAsync<TRequest, TResponse>(this Task<Result<TRequest>> task, Func<TRequest, Task<Result<TResponse>>> func)
    {
        var result = await task;
        if (result.IsFailure)
        {
            return Result<TResponse>.Failure(result.Error);
        }

        return await func(result.Response);
    }
    public static async Task<Result<U>> BindAsync<T, U>(this Result<T> result, Func<T, Task<Result<U>>> func)
    {
        if (result.IsFailure)
        {
            return Result<U>.Failure(result.Error);
        }

        return await func(result.Response);
    }
    #endregion

    #region Tap
    public static Result<T> Tap<T>(this Result<T> result, Action<T> action)
    {
        if (result.IsSuccess)
        {
            action(result.Response);
        }

        return result;
    }
    public static async Task<Result<T>> TapAsync<T>(this Task<Result<T>> task, Action<Result<T>> action)
    {
        var result = await task;
        if (result.IsSuccess)
        {
            action(result);
        }

        return result;
    }
    public static Result<T> TapError<T>(this Result<T> result, Action<IEnumerable<Error>> action)
    {
        if (result.IsFailure)
        {
            action(result.Errors);
        }
        return result;
    }
    public static async Task<Result<T>> TapErrorAsync<T>(this Task<Result<T>> task, Func<IEnumerable<Error>, Task> action)
    {
        var result = await task;
        if (result.IsFailure && result.Errors.Any())
        {
            await action(result.Errors); 
        }
        return result;
    }
    public static async Task<Result<T>> TapErrorAsync<T>(this Task<Result<T>> task, Action<Result<T>> action)
    {
        var result = await task;
        if (result.IsFailure)
        {
            action(result); 
        }
        return result;
    }
    #endregion

    #region TryExecute
    public static Result<T> TryExecute<T>(Func<T> action, Func<Exception, IEnumerable<Error>> errorHandler)
    {
        try
        {
            return Result<T>.Success(action());
        }
        catch (Exception ex)
        {
            return Result<T>.Failure(errorHandler(ex).ToList());
        }
    }
    public static async Task<Result<T>> TryExecuteAsync<T>(Func<Task<Result<T>>> action, Func<Exception, IEnumerable<Error>> errorHandler)
    {
        try
        {
            return await action();
        }
        catch (Exception ex)
        {
            return Result<T>.Failure(errorHandler(ex).ToList());
        }
    }
    #endregion

    #region Combine
    public static Result<(T1, T2)> Combine<T1, T2>(this Result<T1> first, Result<T2> second)
    {
        if (first.IsFailure && second.IsFailure)
        {
            var combinedErrors = first.Errors.Concat(second.Errors).ToList();
            return Result<(T1, T2)>.Failure(combinedErrors);
        }
        
        if (first.IsFailure)
            return Result<(T1, T2)>.Failure(first.Errors);

        if (second.IsFailure)
            return Result<(T1, T2)>.Failure(second.Errors);

        return Result<(T1, T2)>.Success((first.Response, second.Response));
    }
    #endregion
}