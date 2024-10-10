using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.SwaggerGen;
using TaskManagement.Api.Auth;
using TaskManagement.Api.Cache;
using TaskManagement.Api.Features;
using TaskManagement.Api.Health;
using TaskManagement.Api.Middleware;
using TaskManagement.Api.Swagger;
using TaskManagement.Api.Versioning;
using TaskManagement.Common;
using TaskManagement.Common.Interfaces;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddUserSecrets<Program>(
    optional: false,
    reloadOnChange: true);
// it load files in : ~/.microsoft/usersecrets/<user_secrets_id>/secrets.json. It's only for development
//To create the file, left-click on the project > Tool> .Net UserSecret
var config = builder.Configuration;

builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();
builder.Logging.AddFilter("Microsoft.AspNetCore.OutputCaching", LogLevel.Debug);

builder.Services.AddAuth(config);
builder.Services.AddVersioning();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddCacheServices(config);

builder.Services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();
builder.Services.AddSwaggerGen(x => x.OperationFilter<SwaggerDefaultValues>());

builder.Services.TryAddCommonServices();
builder.Services.AddFeatureServices(config);

builder.Services.AddHealthChecks()
    .AddCheck<DatabaseHealthCheck>("DatabaseHealth");

var app = builder.Build();
app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();
app.AddCacheApplication();

app.CreateApiVersionSet();

app.UseMiddleware<LoggingMiddleware>();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(x =>
    {
        foreach (var description in app.DescribeApiVersions())
        {
            x.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json",
                description.GroupName);
        }
    });
}


app.AddApiEndpoints();

app.AddHealthEndpoints();

// It's commented because we don't want to initialize the database anymore on AWS (if the database is local we would use that)
// using (var scope = app.Services.CreateScope())
// {
//     var services = scope.ServiceProvider;
//     var initializers = services.GetServices<IDbInitializer>();
//     await Task.WhenAll(initializers.Select(x => x.InitializeAsync()));
// }

app.Run();