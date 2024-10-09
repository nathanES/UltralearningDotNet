namespace TaskManagement.Api.Features.Tasks.Endpoints;

public static class TaskEndpointExtensions
{
   public static IEndpointRouteBuilder AddTasksEndpoints(this IEndpointRouteBuilder app)
   {
      var taskGroup = app.MapGroup("tasks");
      taskGroup.MapCreateTask();
      taskGroup.MapGetTask();
      taskGroup.MapGetAllTasks();
      taskGroup.MapUpdateTask();
      taskGroup.MapDeleteTask();
      
      return app;
   }
}