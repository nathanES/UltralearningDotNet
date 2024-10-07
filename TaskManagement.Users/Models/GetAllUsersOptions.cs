namespace TaskManagement.Users.Models;

public class GetAllUsersOptions
{
    public string? Username { get; set; }
    public string? Email { get; set; }

    public int Page { get; set; }

    public int PageSize { get; set; }
}