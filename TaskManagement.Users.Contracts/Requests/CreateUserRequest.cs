namespace TaskManagement.Users.Contracts.Requests;

public class CreateUserRequest
{
    public required string Username { get; init; }
    public required string Email { get; init; }  
}