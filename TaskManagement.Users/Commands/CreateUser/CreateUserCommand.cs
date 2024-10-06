using TaskManagement.Common.Mediator;

namespace TaskManagement.Users.Commands.CreateUser;

public class CreateUserCommand(Guid id, string username, string email) : IRequest<bool>
{
    public Guid Id { get; } = id;
    public string Username { get; } = username;
    public string Email { get; } = email;
}