using FluentValidation;
using TaskManagement.Users.Commands.DeleteUser;

namespace TaskManagement.Users.Validators;

public class DeleteUserCommandValidator: AbstractValidator<DeleteUserCommand>
{
    public DeleteUserCommandValidator()
    {
        RuleFor(x => x)
            .NotNull();
        RuleFor(x => x.Id)
            .NotEmpty();
    }
}