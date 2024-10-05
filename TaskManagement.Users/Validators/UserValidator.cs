using FluentValidation;
using TaskManagement.Common.Models;

namespace TaskManagement.Users.Validators;

public class UserValidator : AbstractValidator<User>
{
    public UserValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty();
    }
}