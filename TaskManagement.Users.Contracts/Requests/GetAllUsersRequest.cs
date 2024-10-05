namespace TaskManagement.Users.Contracts.Requests;

public class GetAllUsersRequest : PagedRequest
{
    public string? Username { get; set; }
    public string? Email { get; set; } 
}