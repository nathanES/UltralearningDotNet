using FluentValidation;
using TaskManagement.Users.Commands.CreateUser;

namespace TaskManagement.Users.Validators;

public class CreateUserCommandValidator: AbstractValidator<CreateUserCommand>
{
    public CreateUserCommandValidator()
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