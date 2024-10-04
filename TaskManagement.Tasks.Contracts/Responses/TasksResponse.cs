namespace TaskManagement.Tasks.Contracts.Responses;

public class TasksResponse
{
    public IEnumerable<TaskResponse> Tasks { get; init; } = Enumerable.Empty<TaskResponse>();
}