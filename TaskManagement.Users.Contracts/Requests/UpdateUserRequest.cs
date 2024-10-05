namespace TaskManagement.Users.Contracts.Requests;

public class UpdateUserRequest
{
    public required string Username { get; init; }
    public required string Email { get; init; } 
}