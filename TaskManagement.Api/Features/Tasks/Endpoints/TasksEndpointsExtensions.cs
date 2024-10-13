namespace TaskManagement.Api.Features.Tasks.Endpoints;

public static class TasksEndpointsExtensions
{
   public static IEndpointRouteBuilder AddTasksEndpointsOutputCache(this IEndpointRouteBuilder app,
      IConfiguration config)
   {
      var tasksGroup = app.MapGroup("tasks");
      tasksGroup.MapCreateTaskOutputCache();
      tasksGroup.MapGetTaskOutputCache();
      tasksGroup.MapGetAllTasksOutputCache();
      tasksGroup.MapUpdateTaskOutputCache();
      tasksGroup.MapDeleteTaskOutputCache();
      return app;
   }
   public static IEndpointRouteBuilder AddTasksEndpointsRedisCache(this IEndpointRouteBuilder app,
      IConfiguration config)
   {
      var tasksGroup = app.MapGroup("tasks");
      tasksGroup.MapCreateTaskRedisCache(config);
      tasksGroup.MapGetTaskRedisCache(config);
      tasksGroup.MapGetAllTasksRedisCache(config);
      tasksGroup.MapUpdateTaskRedisCache(config);
      tasksGroup.MapDeleteTaskRedisCache(config);
      return app;
   }
}