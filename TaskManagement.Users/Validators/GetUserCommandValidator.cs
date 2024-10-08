using System.Data;
using FluentValidation;
using TaskManagement.Users.Commands.GetUser;

namespace TaskManagement.Users.Validators;

public class GetUserCommandValidator: AbstractValidator<GetUserCommand>
{
    public GetUserCommandValidator()
    {
        RuleFor(x => x)
            .NotNull();
        RuleFor(x => x.Id)
            .NotEmpty();
    }
}