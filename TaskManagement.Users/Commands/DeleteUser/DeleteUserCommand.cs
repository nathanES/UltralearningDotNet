using TaskManagement.Common.Mediator;

namespace TaskManagement.Users.Commands.DeleteUser;

public class DeleteUserCommand(Guid id) : IRequest<bool>
{
    public Guid Id { get; } = id;
}