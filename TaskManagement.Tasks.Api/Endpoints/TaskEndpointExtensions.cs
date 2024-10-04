namespace TaskManagement.Tasks.Api.Endpoints;

public static class TaskEndpointExtensions
{
   public static IEndpointRouteBuilder MapApiEndpoints(this IEndpointRouteBuilder app)
   {
      app.MapCreateTask();
      app.MapGetTask();
      app.MapGetAllTasks();
      app.MapUpdateTask();
      app.MapDeleteTask();
      return app;
   }
}