using TaskManagement.Common.Mediator;
using TaskManagement.Common.Models;

namespace TaskManagement.Users.Commands.UpdateUser;

public class UpdateUserCommand(Guid id, string username, string email): IRequest<User?>
{
    public Guid Id { get; } = id;
    public string Username { get; } = username;
    public string Email { get; } = email;
}