using AspNetCoreWebApplication;

Host.CreateDefaultBuilder(args)
    .ConfigureHostConfiguration(hostConfig =>
    {
        hostConfig.SetBasePath(Directory.GetCurrentDirectory());
        hostConfig.AddJsonFile("hostsettings.json", optional: true);
        hostConfig.AddEnvironmentVariables(prefix: "PREFIX_");
        hostConfig.AddCommandLine(args);
    });

var builder = WebApplication.CreateBuilder(args);

// Step 1: Register host-wide services (background tasks, generic services)
// builder.Services.AddSingleton<IMyService, MyService>();  // Non-web generic service
builder.Services.AddHostedService<HostApplicationLifetimeEventsHostedService>(); // Background service

// Step 2: Register web-specific services
builder.Services.AddControllers();  // Web service (e.g., MVC controllers)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAuthentication();  // Add authentication

var app = builder.Build();

// Step 3: Configure middleware and HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();  // Include Authorization middleware if needed

app.MapControllers();  // Configure controller routes

// app.Map("/api", apiApp =>
// {
//     apiApp.Map("/movies1", HandleMapTest1);
//     apiApp.Map("/movies2", HandleMapTest2);
// });

app.MapWhen(context => context.Request.Query.ContainsKey("api"), HandleWhen);

app.Run(async context =>
{
    await context.Response.WriteAsync("Hello from non-Map delegate.");
});
// Step 4: Run the unified application (handles both the web server and background services)
app.Run();

static void HandleWhen(IApplicationBuilder app)
{
    app.Run(async context =>
    {
        await context.Response.WriteAsync("Map When api");
    }); 
}
static void HandleMapTest1(IApplicationBuilder app)
{
    app.Run(async context =>
    {
        await context.Response.WriteAsync("Map Test 1 Nested");
    });
}

static void HandleMapTest2(IApplicationBuilder app)
{
    app.Run(async context =>
    {
        await context.Response.WriteAsync("Map Test 2 Nested");
    });
}