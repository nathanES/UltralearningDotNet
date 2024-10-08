namespace TaskManagement.Common.Interfaces.Commands;

public interface IShouldCheckUserExistenceCommand
{
    Guid? UserId { get; }
}