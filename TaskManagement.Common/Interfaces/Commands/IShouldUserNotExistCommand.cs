namespace TaskManagement.Common.Interfaces.Commands;

public interface IShouldUserNotExistCommand
{
    Guid? UserId { get; }
}
public interface IShouldUserNotExistUserCommand
{
    Guid Id { get; }
}