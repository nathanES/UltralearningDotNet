namespace TaskManagement.Api.Health;

public static class HealthExtension
{
   public static IApplicationBuilder AddHealthEndpoints(this IApplicationBuilder app)
   {
     //app.UseRouting(); It should be called before 

      app.UseEndpoints(endpoints =>
      {
         endpoints.MapHealthChecks("/health");
      });
      return app;
   }
}