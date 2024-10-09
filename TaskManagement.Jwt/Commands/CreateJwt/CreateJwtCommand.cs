using TaskManagement.Common.Middleware;
using TaskManagement.Common.ResultPattern;

namespace TaskManagement.Jwt.Commands.CreateJwt;

public class CreateJwtCommand : IRequest<Result<string>>
{
    public Guid UserId { get; set; }
    public string Email { get; set; }
    public Dictionary<string, Object> CustomClaims { get; set; } = new();
}