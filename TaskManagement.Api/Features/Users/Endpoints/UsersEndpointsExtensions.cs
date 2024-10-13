namespace TaskManagement.Api.Features.Users.Endpoints;

public static class UsersEndpointsExtensions
{
   public static IEndpointRouteBuilder AddUsersEndpointsOutputCache(this IEndpointRouteBuilder app,
      IConfiguration config)
   {
      var usersGroup = app.MapGroup("users");
      usersGroup.MapCreateUserOutputCache();
      usersGroup.MapGetUserOutputCache();
      usersGroup.MapGetAllUsersOutputCache();
      usersGroup.MapUpdateUserOutputCache();
      usersGroup.MapDeleteUserOutputCache();
      return app;
   }
   public static IEndpointRouteBuilder AddUsersEndpointsRedisCache(this IEndpointRouteBuilder app,
      IConfiguration config)
   {
      var usersGroup = app.MapGroup("users");
      usersGroup.MapCreateUserRedisCache(config);
      usersGroup.MapGetUserRedisCache(config);
      usersGroup.MapGetAllUsersRedisCache(config);
      usersGroup.MapUpdateUserRedisCache(config);
      usersGroup.MapDeleteUserRedisCache(config);
      return app;
   }
}