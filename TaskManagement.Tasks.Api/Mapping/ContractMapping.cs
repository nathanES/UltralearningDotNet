using TaskManagement.Common.Models;
using TaskManagement.Tasks.Commands.CreateTask;
using TaskManagement.Tasks.Contracts.Requests;
using TaskManagement.Tasks.Contracts.Responses;
using TaskManagement.Tasks.Models;
using Task = TaskManagement.Common.Models.Task;

namespace TaskManagement.Tasks.Api.Mapping;

public static class ContractMapping
{
    public static CreateTaskCommand MapToCommand(this CreateTaskRequest request)
    {
        return new CreateTaskCommand(Guid.NewGuid(),
            request.Title,
            request.Description,
            request.DeadLine,
            request.Priority.MapToPriorityModels(),
            request.Status.MapToStatusModels()); 
    }
    public static Task MapToTask(this UpdateTaskRequest request, Guid id)
    {
        return new Task(id,
            request.Title,
            request.Description,
            request.DeadLine,
            request.Priority.MapToPriorityModels(),
            request.Status.MapToStatusModels());
    }

    public static Models.GetAllTasksOptions MapToGetAllTasksOptions(this GetAllTasksRequest request)
    {
        return new GetAllTasksOptions
        {
            Title = request.Title,
            Description = request.Title,
            DeadLine = request.DeadLine,
            Priority =  request.Priority.MapToPriorityModels(),
            Status = request.Status.MapToStatusModels(),
            Page = request.Page.GetValueOrDefault(PagedRequest.DefaultPage),
            PageSize = request.PageSize.GetValueOrDefault(PagedRequest.DefaultPageSize)
        };
    }
    
    
    public static TaskResponse MapToResponse(this Task task)
    {
        return new TaskResponse
        {
            Id = task.Id,
            Title = task.Title,
            Description = task.Description,
            DeadLine = task.DeadLine,
            Priority = task.Priority.MapToPriorityContract(),
            Status = task.Status.MapToStatusContract()
        };
    }
    public static TasksResponse MapToResponse(this IEnumerable<Task> tasks)
    {
        return new TasksResponse
        {
            Tasks = tasks.Select(MapToResponse),
        };
    }

    private static Priority? MapToPriorityModels(this Contracts.Priority? priority)
    {
        return priority switch
        {
            Contracts.Priority.Low => Priority.Low,
            Contracts.Priority.Medium => Priority.Medium,
            Contracts.Priority.High => Priority.High,
            _ => null
        };
    }
    private static Status? MapToStatusModels(this Contracts.Status? status)
    {
        return status switch
        {
            Contracts.Status.Open => Status.Open,
            Contracts.Status.InProgress => Status.InProgress,
            Contracts.Status.Closed => Status.Closed,
            _ => null
        };
    }
    
    private static Contracts.Priority? MapToPriorityContract(this Priority? priority)
    {
        return priority switch
        {
            Priority.Low => Contracts.Priority.Low,
            Priority.Medium => Contracts.Priority.Medium,
            Priority.High => Contracts.Priority.High,
            _ => null
        };
    }
    private static Contracts.Status? MapToStatusContract(this Status? status)
    {
        return status switch
        {
            Status.Open => Contracts.Status.Open,
            Status.InProgress =>  Contracts.Status.InProgress,
            Status.Closed => Contracts.Status.Closed,
            _ => null
        };
    }
}