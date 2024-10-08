using TaskManagement.Common.Models;
using TaskManagement.Common.ResultPattern;
using TaskManagement.Users.Models;

namespace TaskManagement.Users.Interfaces;

public interface IUserRepository
{
    Task<Result<User>> CreateAsync(User user, CancellationToken token = default);
    Task<Result<User>> GetByIdAsync(Guid id, CancellationToken token = default);
    Task<Result<IEnumerable<User>>> GetAllAsync(GetAllUsersOptions options, CancellationToken token = default);
    Task<Result<User>> UpdateAsync(User user, CancellationToken token = default);
    Task<Result<None>> DeleteByIdAsync(Guid id, CancellationToken token = default);
    Task<Result<bool>> ExistsByIdAsync(Guid id, CancellationToken token = default); 
}