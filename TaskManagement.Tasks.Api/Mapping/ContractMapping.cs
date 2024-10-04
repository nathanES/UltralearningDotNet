using TaskManagement.Tasks.Contracts.Requests;
using TaskManagement.Tasks.Contracts.Responses;
using TaskManagement.Tasks.Models;

namespace TaskManagement.Tasks.Api.Mapping;

public static class ContractMapping
{
    public static Models.Task MapToTask(this CreateTaskRequest request)
    {
        Guid id = Guid.NewGuid();
        return new Models.Task(id,request.Title,
            request.Description,
            request.DeadLine,
            request.Priority.MapToPriorityModels(),
            request.Status.MapToStatusModels());
    }
    public static Models.Task MapToTask(this UpdateTaskRequest request, Guid id)
    {
        return new Models.Task(id,
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
            SortOrder = null,//TODO
            Page = request.Page.GetValueOrDefault(PagedRequest.DefaultPage),
            PageSize = request.PageSize.GetValueOrDefault(PagedRequest.DefaultPageSize)
        };
    }
    
    
    public static TaskResponse MapToResponse(this Models.Task task)
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
    public static TasksResponse MapToResponse(this IEnumerable<Models.Task> tasks)
    {
        return new TasksResponse
        {
            Tasks = tasks.Select(MapToResponse),
        };
    }

    private static Models.Priority? MapToPriorityModels(this Contracts.Priority? priority)
    {
        return priority switch
        {
            Contracts.Priority.Low => Models.Priority.Low,
            Contracts.Priority.Medium => Models.Priority.Medium,
            Contracts.Priority.High => Models.Priority.High,
            _ => null
        };
    }
    private static Models.Status? MapToStatusModels(this Contracts.Status? status)
    {
        return status switch
        {
            Contracts.Status.Open => Models.Status.Open,
            Contracts.Status.InProgress => Models.Status.InProgress,
            Contracts.Status.Closed => Models.Status.Closed,
            _ => null
        };
    }
    
    private static Contracts.Priority? MapToPriorityContract(this Models.Priority? priority)
    {
        return priority switch
        {
            Models.Priority.Low => Contracts.Priority.Low,
            Models.Priority.Medium => Contracts.Priority.Medium,
            Models.Priority.High => Contracts.Priority.High,
            _ => null
        };
    }
    private static Contracts.Status? MapToStatusContract(this Models.Status? status)
    {
        return status switch
        {
            Models.Status.Open => Contracts.Status.Open,
            Models.Status.InProgress =>  Contracts.Status.InProgress,
            Models.Status.Closed => Contracts.Status.Closed,
            _ => null
        };
    }
}