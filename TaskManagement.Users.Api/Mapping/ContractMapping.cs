using TaskManagement.Common.Models;
using TaskManagement.Users.Contracts.Requests;
using TaskManagement.Users.Contracts.Responses;
using TaskManagement.Users.Models;
using Task = TaskManagement.Common.Models.Task;

namespace TaskManagement.Users.Api.Mapping;

public static class ContractMapping
{
    public static User MapToUser(this CreateUserRequest request)
    {
        Guid id = Guid.NewGuid();
        return new User(id, request.Username,
            request.Email);
    }
    public static User MapToUser(this UpdateUserRequest request, Guid id)
    {
        return new User(id,
            request.Username,
            request.Email);
    }

    public static GetAllUsersOptions MapToGetAllUsersOptions(this GetAllUsersRequest request)
    {
        return new GetAllUsersOptions
        {
            Username = request.Username,
            Email = request.Email,
            Page = request.Page.GetValueOrDefault(PagedRequest.DefaultPage),
            PageSize = request.PageSize.GetValueOrDefault(PagedRequest.DefaultPageSize)
        };
    }
    
    
    public static UserResponse MapToResponse(this User user)
    {
        return new UserResponse
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email,
        };
    }
    public static UsersResponse MapToResponse(this IEnumerable<User> users)
    {
        return new UsersResponse
        {
            Users = users.Select(MapToResponse),
        };
    }
}