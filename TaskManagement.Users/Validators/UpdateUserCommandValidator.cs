using FluentValidation;
using TaskManagement.Users.Commands.UpdateUser;

namespace TaskManagement.Users.Validators;

public class UpdateUserCommandValidator: AbstractValidator<UpdateUserCommand>
{
    public UpdateUserCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty();
        RuleFor(x => x.Username)
            .NotEmpty()
            .NotNull()
            .MaximumLength(255).WithMessage("UserName cannot be longer than 255 characters.");
        RuleFor(x => x.Email)
            .NotEmpty()
            .NotNull()
            .MaximumLength(255).WithMessage("Email cannot be longer than 255 characters.");
    }
}