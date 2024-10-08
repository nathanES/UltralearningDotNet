using TaskManagement.Jwt.Commands;
using TaskManagement.Jwt.Commands.CreateJwt;
using TaskManagement.Jwt.Contracts.Requests;

namespace TaskManagement.Api.Features.JWT.Mapping;

public static class ContractMapping
{
    public static CreateJwtCommand MapToCommand(this CreateJwtRequest request)
    {
        return new CreateJwtCommand
        {
            UserId = request.UserId,
            Email = request.Email,
            Username = request.Username,
            CustomClaims = request.CustomClaims
        };
    }
}