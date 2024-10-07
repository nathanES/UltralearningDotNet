using TaskManagement.Common.Models;
using TaskManagement.Tasks.Commands.CreateTask;
using TaskManagement.Tasks.Commands.DeleteTask;
using TaskManagement.Tasks.Commands.GetAllTasks;
using TaskManagement.Tasks.Commands.UpdateTask;
using TaskManagement.Tasks.Contracts.Requests;
using TaskManagement.Tasks.Contracts.Responses;
using Task = TaskManagement.Common.Models.Task;
using PriorityContract = TaskManagement.Tasks.Contracts.Priority;
using StatusContract = TaskManagement.Tasks.Contracts.Status;

namespace TaskManagement.Api.Features.Tasks.Mapping;

public static class ContractMapping
{
    public static CreateTaskCommand MapToCommand(this CreateTaskRequest request)
    {
        return new CreateTaskCommand(Guid.NewGuid(),
            request.Title,
            request.Description,
            request.Deadline,
            request.Priority.MapToPriorityModels(),
            request.Status.MapToStatusModels(),
            request.UserId
            );
    }

    public static GetAllTasksCommand MapToCommand(this GetAllTasksRequest request)
    {
        return new GetAllTasksCommand(
            title: request.Title,
            description: request.Description,
            deadline: request.Deadline,
            priority: request.Priority.MapToPriorityModels(),
            status: request.Status.MapToStatusModels(),
            page: request.Page.GetValueOrDefault(PagedRequest.DefaultPage),
            pageSize: request.PageSize.GetValueOrDefault(PagedRequest.DefaultPageSize)
        );
    }

    public static UpdateTaskCommand MapToCommand(this UpdateTaskRequest request, Guid id)
    {
        return new UpdateTaskCommand(
            id: id,
            title: request.Title,
            description: request.Description,
            deadline: request.Deadline,
            priority: request.Priority.MapToPriorityModels(),
            status: request.Status.MapToStatusModels(),
            userId: request.UserId
        );
    }

    public static TaskResponse MapToResponse(this Task task)
    {
        return new TaskResponse(
            id: task.Id,
            title: task.Title,
            description: task.Description,
            deadline: task.Deadline,
            priority: task.Priority.MapToPriorityContract(),
            status: task.Status.MapToStatusContract(),
            userId: task.UserId
        );
    }

    public static TasksResponse MapToResponse(this IEnumerable<Task> tasks)
    {
        return new TasksResponse
        {
            Tasks = tasks.Select(MapToResponse),
        };
    }

    private static Priority? MapToPriorityModels(this PriorityContract? priority)
    {
        return priority switch
        {
            PriorityContract.Low => Priority.Low,
            PriorityContract.Medium => Priority.Medium,
            PriorityContract.High => Priority.High,
            _ => null
        };
    }

    private static Status? MapToStatusModels(this StatusContract? status)
    {
        return status switch
        {
            StatusContract.Open => Status.Open,
            StatusContract.InProgress => Status.InProgress,
            StatusContract.Closed => Status.Closed,
            _ => null
        };
    }

    private static PriorityContract? MapToPriorityContract(this Priority? priority)
    {
        return priority switch
        {
            Priority.Low => PriorityContract.Low,
            Priority.Medium => PriorityContract.Medium,
            Priority.High => PriorityContract.High,
            _ => null
        };
    }

    private static StatusContract? MapToStatusContract(this Status? status)
    {
        return status switch
        {
            Status.Open => StatusContract.Open,
            Status.InProgress => StatusContract.InProgress,
            Status.Closed => StatusContract.Closed,
            _ => null
        };
    }
}