using TaskManagement.Common.Models;
using TaskManagement.Users.Models;

namespace TaskManagement.Users.Interfaces;

public interface IUserRepository
{
    Task<bool> CreateAsync(User user, CancellationToken token = default);
    Task<User?> GetByIdAsync(Guid id, CancellationToken token = default);
    Task<IEnumerable<User>> GetAllAsync(GetAllUsersOptions options, CancellationToken token = default);
    Task<bool> UpdateAsync(User user, CancellationToken token = default);
    Task<bool> DeleteByIdAsync(Guid id, CancellationToken token = default);
    Task<bool> ExistsByIdAsync(Guid id, CancellationToken token = default); 
}