namespace TaskManagement.Jwt.Contracts.Requests;

public class CreateJwtRequest
{
    public Guid UserId { get; set; }
    public string Email { get; set; }
    public string Username { get; set; }
    public Dictionary<string, Object> CustomClaims { get; set; } = new();
}