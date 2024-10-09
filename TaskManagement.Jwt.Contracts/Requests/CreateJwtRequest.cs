namespace TaskManagement.Jwt.Contracts.Requests;

public class CreateJwtRequest
{
    public Guid UserId { get; set; }
    public string Email { get; set; }
    public string Username { get; set; }
    public bool IsAdmin { get; set; }
    public bool IsTrustedMember { get; set; }
}