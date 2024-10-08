namespace TaskManagement.Common.Interfaces.Commands;

public interface IShouldCheckTaskExistenceCommand
{
    Guid? TaskId { get; }
}