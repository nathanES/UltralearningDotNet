using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using TaskManagement.Common.Middleware;
using TaskManagement.Common.ResultPattern;
using TaskManagement.Common.ResultPattern.Errors;
using TaskManagement.Jwt.Configurations;

namespace TaskManagement.Jwt.Commands.CreateJwt;

internal class CreateJwtHandler(IOptions<JwtSettings> jwtSettingsOptions, ILogger<CreateJwtHandler> logger)
    : IRequestHandler<CreateJwtCommand, Result<string>>
{
    private readonly JwtSettings _jwtSettings = jwtSettingsOptions.Value; 

    public async Task<Result<string>> HandleAsync(CreateJwtCommand request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_jwtSettings.Secret ??
                                             throw new InvalidOperationException("JWT Secret not configured"));

            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new(JwtRegisteredClaimNames.Sub, request.Email),
                new(JwtRegisteredClaimNames.Email, request.Email),
                new("userid", request.UserId.ToString()),
            };

            foreach (var claimPair in request.CustomClaims)
            {
                string claimValue;
                string valueType;

                // Check if the value is "True" or "False" (case-insensitive)
                if (bool.TryParse(claimPair.Value.ToString(), out bool boolValue))
                {
                    claimValue = boolValue.ToString().ToLower(); // Ensure lowercase 'true'/'false'
                    valueType = ClaimValueTypes.Boolean;
                }
                else
                {
                    // Treat the value as a string
                    claimValue = claimPair.Value.ToString();
                    valueType = ClaimValueTypes.String;
                }

                if (claimValue is null)
                    continue;
                
                var claim = new Claim(claimPair.Key, claimValue, valueType);
                claims.Add(claim);
            }

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.Add(
                    TimeSpan.FromHours(_jwtSettings.TokenLifetimeHours)),
                Issuer = _jwtSettings.Issuer,
                Audience = _jwtSettings.Audience,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            var jwt = tokenHandler.WriteToken(token);
            return Result<string>.Success(jwt);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to generate JWT");
            return Result<string>.Failure(new GenericError("JWT Generation Error", details:ex.Message));
        }
    }
}