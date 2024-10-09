namespace TaskManagement.Api.Features.Users.Endpoints;

public static class UserEndpointExtensions
{
   public static IEndpointRouteBuilder AddUsersEndpoints(this IEndpointRouteBuilder app)
   {
      var usersGroup = app.MapGroup("users");
      usersGroup.MapCreateUser();
      usersGroup.MapGetUser();
      usersGroup.MapGetAllUsers();
      usersGroup.MapUpdateUser();
      usersGroup.MapDeleteUser();
      return app;
   }
}