using FluentValidation;
using TaskManagement.Users.Commands.GetAllUsers;

namespace TaskManagement.Users.Validators;

public class GetAllUsersCommandValidator: AbstractValidator<GetAllUsersCommand>
{
    public GetAllUsersCommandValidator()
    {
        RuleFor(x => x)
            .NotNull();
        RuleFor(x => x.Page)
            .GreaterThan(0).WithMessage("Page number must be greater than 0.");
        
        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100).WithMessage("Page size must be between 1 and 100."); 
    }
}