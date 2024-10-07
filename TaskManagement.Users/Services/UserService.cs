using FluentValidation;
using Microsoft.Extensions.Logging;
using TaskManagement.Common.Models;
using TaskManagement.Common.ResultPattern;
using TaskManagement.Common.ResultPattern.Errors;
using TaskManagement.Users.Interfaces;
using TaskManagement.Users.Models;

namespace TaskManagement.Users.Services;

public class UserService(IUserRepository userRepository,  ILogger<UserService> logger): IUserService
{
    public async Task<Result<User>> CreateAsync(User user, CancellationToken token = default)
    {
        var isUserExistResult =await userRepository.ExistsByIdAsync(user.Id, token);
        if (isUserExistResult.IsFailure)
        {
            logger.LogWarning("A technical error occurred while checking for user existence");
            return Result<User>.Failure(isUserExistResult.Errors);
        }
        return await userRepository.CreateAsync(user, token);
    }

    public async Task<Result<User>> GetByIdAsync(Guid id, CancellationToken token = default)
    {
        return await userRepository.GetByIdAsync(id, token);
    }

    public async Task<Result<IEnumerable<User>>> GetAllAsync(GetAllUsersOptions options, CancellationToken token = default)
    {
        return await userRepository.GetAllAsync(options, token);
    }

    public async Task<Result<User>> UpdateAsync(User user, CancellationToken token = default)
    {
        var isUserExistResult = await userRepository.ExistsByIdAsync(user.Id, token);
        if (isUserExistResult.IsFailure)
        {
            logger.LogWarning("A technical error occurred while checking for user existence");
            return Result<User>.Failure(isUserExistResult.Errors);
        }
        if (!isUserExistResult.Response)
        {
            logger.LogWarning("User does not exist");
            return Result<User>.Failure(new ConflictError("User does not exist", "User", user.Id.ToString()));
        } 
        var updateUserResult = await userRepository.UpdateAsync(user, token);
        if (updateUserResult.IsFailure)
        {
            logger.LogWarning("A technical error occurred while updating user");
            return Result<User>.Failure(updateUserResult.Errors);
        }
        
        return Result<User>.Success(updateUserResult.Response); 
    }

    public async Task<Result<None>> DeleteByIdAsync(Guid id, CancellationToken token = default)
    {
        return await userRepository.DeleteByIdAsync(id, token);
    }

    public async Task<Result<bool>> ExistAsync(Guid id, CancellationToken token = default)
    {
        return await userRepository.ExistsByIdAsync(id, token);
    }
}