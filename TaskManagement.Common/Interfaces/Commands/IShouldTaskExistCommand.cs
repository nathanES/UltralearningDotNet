namespace TaskManagement.Common.Interfaces.Commands;

public interface IShouldTaskExistCommand
{
    Guid? TaskId { get; }
}
public interface IShouldTaskExistTaskCommand
{
    Guid Id { get; }
}

