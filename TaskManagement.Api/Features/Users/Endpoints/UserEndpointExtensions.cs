namespace TaskManagement.Api.Features.Users.Endpoints;

public static class UserEndpointExtensions
{
   public static IEndpointRouteBuilder AddUsersEndpoints(this IEndpointRouteBuilder app)
   {
      app.MapCreateUser();
      app.MapGetUser();
      app.MapGetAllUsers();
      app.MapUpdateUser();
      app.MapDeleteUser();
      return app;
   }
}