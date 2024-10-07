using TaskManagement.Common.Models;
using TaskManagement.Users.Commands.CreateUser;
using TaskManagement.Users.Commands.GetAllUsers;
using TaskManagement.Users.Commands.UpdateUser;
using TaskManagement.Users.Contracts.Requests;
using TaskManagement.Users.Contracts.Responses;

namespace TaskManagement.Api.Features.Users.Mapping;

public static class ContractMapping
{
    public static CreateUserCommand MapToCommand(this CreateUserRequest request)
    {
        return new CreateUserCommand(Guid.NewGuid(), request.Username,
            request.Email);
    }
    public static UpdateUserCommand MapToCommand(this UpdateUserRequest request, Guid id)
    {
        return new UpdateUserCommand(id,
            request.Username,
            request.Email);
    }

    public static GetAllUsersCommand MapToCommand(this GetAllUsersRequest request)
    {
        return new GetAllUsersCommand(request.Username,
            request.Email,
            request.Page.GetValueOrDefault(PagedRequest.DefaultPage),
            request.PageSize.GetValueOrDefault(PagedRequest.DefaultPageSize)); 
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