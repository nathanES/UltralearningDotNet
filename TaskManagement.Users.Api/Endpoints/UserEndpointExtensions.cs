namespace TaskManagement.Users.Api.Endpoints;

public static class UserEndpointExtensions
{
   public static IEndpointRouteBuilder MapApiEndpoints(this IEndpointRouteBuilder app)
   {
      app.MapCreateUser();
      app.MapGetUser();
      app.MapGetAllUsers();
      app.MapUpdateUser();
      app.MapDeleteUser();
      return app;
   }
}