namespace TaskManagement.Common.Interfaces.Commands;

public interface IShouldTaskNotExistCommand
{
    Guid? TaskId { get; }
}

public interface IShouldTaskNotExistTaskCommand
{
    Guid Id { get; }
}