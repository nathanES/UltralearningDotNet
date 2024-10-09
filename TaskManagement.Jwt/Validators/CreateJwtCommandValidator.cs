using FluentValidation;
using TaskManagement.Common.Models;
using TaskManagement.Jwt.Commands.CreateJwt;

namespace TaskManagement.Jwt.Validators;

public class CreateJwtCommandValidator : AbstractValidator<CreateJwtCommand>
{
    public CreateJwtCommandValidator()
    {
        RuleFor(x => x)
            .NotNull();
        RuleFor(x => x.UserId)
            .NotNull();
        RuleFor(x => x.Email)
            .NotNull()
            .NotEmpty();
    }
}