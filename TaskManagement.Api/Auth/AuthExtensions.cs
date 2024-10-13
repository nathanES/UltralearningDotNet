using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace TaskManagement.Api.Auth;

public static class AuthExtensions
{
   public static IServiceCollection AddAuthServices(this IServiceCollection services, IConfiguration config)
   {
      services.AddAuthentication(x =>
      {
         x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
         x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
         x.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
      }).AddJwtBearer(x =>
      {
         x.TokenValidationParameters = new TokenValidationParameters
         {
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Secret"]!)),
            ValidateIssuerSigningKey = true,
            ValidateLifetime = true,
            ValidIssuer = config["Jwt:Issuer"],
            ValidateIssuer = true,
            ValidAudience = config["Jwt:Audience"],
            ValidateAudience = true
         };
         x.Events = new JwtBearerEvents
         {
            OnAuthenticationFailed = context =>
            {
               if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
               {
                  context.Response.Headers.Append("Token-Expired", "true");
               }
               return Task.CompletedTask;
            }
         };
      });
      services.AddAuthorization(x =>
      {
         x.AddPolicy(AuthConstants.AdminUserPolicyName,
            policy => policy.AddRequirements(new AdminAuthRequirement(config["ApiKey"]!)));
    
         x.AddPolicy(AuthConstants.TrustedMemberPolicyName,
            p => p.RequireAssertion(c => 
               c.User.HasClaim(m => m is { Type: AuthConstants.AdminUserClaimName, Value: "true" }) || 
               c.User.HasClaim(m => m is { Type: AuthConstants.TrustedMemberClaimName, Value: "true" }))); 
      });

      services.AddScoped<ApiKeyAuthFilter>();
      return services;
   }
}