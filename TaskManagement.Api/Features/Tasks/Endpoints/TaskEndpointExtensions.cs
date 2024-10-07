namespace TaskManagement.Api.Features.Tasks.Endpoints;

public static class TaskEndpointExtensions
{
   public static IEndpointRouteBuilder AddTasksEndpoints(this IEndpointRouteBuilder app)
   {
      app.MapCreateTask();
      app.MapGetTask();
      app.MapGetAllTasks();
      app.MapUpdateTask();
      app.MapDeleteTask();
      return app;
   }
}