using Asp.Versioning;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.SwaggerGen;
using TaskManagement.Common;
using TaskManagement.Common.Mediator;
using TaskManagement.Tasks;
using TaskManagement.Tasks.Api;
using TaskManagement.Tasks.Api.Endpoints;
using TaskManagement.Tasks.Api.Middleware;
using TaskManagement.Tasks.Api.Swagger;
using TaskManagement.Tasks.Commands.CreateTask;
using TaskManagement.Tasks.Database;
using TaskManagement.Tasks.Infrastructure;

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;

// Logging : By default there is a default ILogger already registered and it used the appsettings.json to log
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

builder.Services.AddApiVersioning(x =>
{
    x.DefaultApiVersion = new ApiVersion(1.0);
    x.AssumeDefaultVersionWhenUnspecified = true;
    x.ReportApiVersions = true;
    x.ApiVersionReader = new MediaTypeApiVersionReader("api-version");
    //Header of the request : "Accept" : "application/json;api-version=1.0"
}).AddApiExplorer();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();
builder.Services.AddSwaggerGen(x => x.OperationFilter<SwaggerDefaultValues>());


builder.Services.TryAddCommonServices();
builder.Services.AddInfrastructure(config["Database:Task:ConnectionString"]!);
builder.Services.AddTaskApplication();



var app = builder.Build();
app.UseMiddleware<LoggingMiddleware>();
app.CreateApiVersionSet();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{  
    app.UseSwagger();
    app.UseSwaggerUI(x =>
    {
        foreach (var description in app.DescribeApiVersions())
        {
            x.SwaggerEndpoint( $"/swagger/{description.GroupName}/swagger.json",
                description.GroupName);
        }
    });
}

app.UseHttpsRedirection();

app.MapApiEndpoints();
// app.Use(async (context, next) =>
// {
//     Console.WriteLine($"Request Path: {context.Request.Path}");
//     await next.Invoke();
//     Console.WriteLine($"Response Status Code: {context.Response.StatusCode}");
//     // Do logging or other work that doesn't write to the Response.
// });
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var initializer = services.GetRequiredService<IDbInitializer>();

    // Initialize the database (apply migrations)
    await initializer.InitializeAsync();
}

app.Run();