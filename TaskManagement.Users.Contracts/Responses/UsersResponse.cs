namespace TaskManagement.Users.Contracts.Responses;

public class UsersResponse
{
    public IEnumerable<UserResponse> Users { get; init; } = Enumerable.Empty<UserResponse>();
}