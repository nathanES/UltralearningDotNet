using FluentValidation;
using Microsoft.Extensions.Logging;
using TaskManagement.Common.Models;
using TaskManagement.Users.Interfaces;
using TaskManagement.Users.Models;

namespace TaskManagement.Users.Services;

public class UserService(IUserRepository userRepository, IValidator<User> validator, ILogger<UserService> loger): IUserService
{
    public async Task<bool> CreateAsync(User user, CancellationToken token = default)
    {
        await validator.ValidateAndThrowAsync(user, token);
        return await userRepository.CreateAsync(user, token);
    }

    public async Task<User?> GetByIdAsync(Guid id, CancellationToken token = default)
    {
        return await userRepository.GetByIdAsync(id, token);
    }

    public async Task<IEnumerable<User>> GetAllAsync(GetAllUsersOptions options, CancellationToken token = default)
    {
        return await userRepository.GetAllAsync(options, token);
    }

    public async Task<User?> UpdateAsync(User user, CancellationToken token = default)
    {
        await validator.ValidateAndThrowAsync(user, token);

        var isTaskExisting = await userRepository.ExistsByIdAsync(user.Id, token);
        if (!isTaskExisting)
            return null;
        await userRepository.UpdateAsync(user, token);
        return user;
    }

    public async Task<bool> DeleteByIdAsync(Guid id, CancellationToken token = default)
    {
        return await userRepository.DeleteByIdAsync(id, token);
    }
}