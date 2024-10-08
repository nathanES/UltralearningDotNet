using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace TaskManagement.Api.Auth;

public class AdminAuthRequirement(string apiKey) : IAuthorizationHandler, IAuthorizationRequirement
{
    public Task HandleAsync(AuthorizationHandlerContext context)
    {
        if (context.User.HasClaim(AuthConstants.AdminUserClaimName, "true"))
        {
            context.Succeed(this);
            return Task.CompletedTask;
        }
        
        var httpContext = context.Resource as HttpContext;
        if (httpContext is null)
        {
            return Task.CompletedTask;
        }
        
        if (!httpContext.Request.Headers.TryGetValue(AuthConstants.ApiKeyHeaderName, out
                var extractedApiKey))
        {
            context.Fail();
            return Task.CompletedTask;
        }
        
        if (apiKey != extractedApiKey)
        {
            context.Fail();
            return Task.CompletedTask;
        }

        var identity = (ClaimsIdentity)httpContext.User.Identity!;
        identity.AddClaim(new Claim("userid", Guid.Parse("74e24de1-8dd0-4bc2-a9f5-8aa3203ad209").ToString()));
        context.Succeed(this);
        return Task.CompletedTask;
    }
}