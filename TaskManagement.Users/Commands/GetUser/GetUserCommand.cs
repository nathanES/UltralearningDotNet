using TaskManagement.Common.Mediator;
using TaskManagement.Common.Models;

namespace TaskManagement.Users.Commands.GetUser;

public class GetUserCommand(Guid id) : IRequest<User?>
{
    public Guid Id { get; } = id;
}