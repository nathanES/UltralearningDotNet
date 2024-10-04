using System.Text;

namespace TaskManagement.Tasks.Api.Middlewares;

public class LoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<LoggingMiddleware> _logger;

    public LoggingMiddleware(RequestDelegate next, ILogger<LoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }
    public async Task InvokeAsync(HttpContext context)
    {
        // Log query parameters
        if (context.Request.Query.Any())
        {
            var queryParams = context.Request.Query;
            foreach (var param in queryParams)
            {
                _logger.LogInformation($"Query Param: {param.Key} = {param.Value}");
            }
        }

        // Log route parameters
        var routeValues = context.Request.RouteValues;
        if (routeValues.Any())
        {
            foreach (var param in routeValues)
            {
                _logger.LogInformation($"Route Param: {param.Key} = {param.Value}");
            }
        }

        // Log body parameters (for POST/PUT)
        if (context.Request.Method == HttpMethods.Post || context.Request.Method == HttpMethods.Put)
        {
            context.Request.EnableBuffering();  // Enable request body for multiple reads

            using (var reader = new StreamReader(context.Request.Body, Encoding.UTF8, detectEncodingFromByteOrderMarks: false, leaveOpen: true))
            {
                var bodyContent = await reader.ReadToEndAsync();
                context.Request.Body.Position = 0;  // Reset the stream position after reading

                if (!string.IsNullOrEmpty(bodyContent))
                {
                    _logger.LogInformation($"Request Body: {bodyContent}");
                }
            }
        }

        // Save the original response body stream
        var originalBodyStream = context.Response.Body;

        using (var responseBody = new MemoryStream())
        {
            // Replace the response body with a memory stream to capture the response
            context.Response.Body = responseBody;

            // Call the next middleware in the pipeline
            await _next(context);

            // Read the response from the memory stream
            responseBody.Seek(0, SeekOrigin.Begin);
            var responseText = await new StreamReader(responseBody).ReadToEndAsync();
            responseBody.Seek(0, SeekOrigin.Begin);

            // Log the response body
            _logger.LogInformation($"Response: {responseText}");

            // Copy the response body back to the original stream
            await responseBody.CopyToAsync(originalBodyStream);
        }
    }
}