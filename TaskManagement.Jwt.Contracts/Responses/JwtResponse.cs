namespace TaskManagement.Jwt.Contracts.Responses;

public class JwtResponse(Guid id, string jwt)
{
    public Guid Id { get; } = id;
    public string Jwt { get; } = jwt;
}