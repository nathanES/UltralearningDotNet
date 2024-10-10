namespace TaskManagement.Common.Interfaces.Commands;

public interface IShouldUserExistCommand
{
    Guid? UserId { get; }
}
public interface IShouldUserExistUserCommand
{
    Guid Id { get; }
}