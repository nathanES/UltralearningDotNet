using TaskManagement.Common.Models;
using TaskManagement.Users.Models;

namespace TaskManagement.Users.Services;

public interface IUserService
{
    Task<bool> CreateAsync(User user, CancellationToken token = default);
    Task<User?> GetByIdAsync(Guid id, CancellationToken token = default);
    Task<IEnumerable<User>> GetAllAsync(GetAllUsersOptions options, CancellationToken token = default);
    Task<User?> UpdateAsync(User user, CancellationToken token = default);
    Task<bool> DeleteByIdAsync(Guid id, CancellationToken token = default); 
}