using FluentValidation;
using TaskManagement.Common.Models;
using TaskManagement.Users.Models;

namespace TaskManagement.Users.Validators;

public class UserValidator : AbstractValidator<User>
{
    public UserValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty();
    }
}