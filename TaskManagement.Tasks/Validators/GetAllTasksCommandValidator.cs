using FluentValidation;
using TaskManagement.Tasks.Commands.GetAllTasks;

namespace TaskManagement.Tasks.Validators;

public class GetAllTasksCommandValidator : AbstractValidator<GetAllTasksCommand>
{
    public GetAllTasksCommandValidator()
    {
        RuleFor(x => x)
            .NotNull();
        RuleFor(x => x.Page)
            .GreaterThan(0).WithMessage("Page number must be greater than 0.");
        
        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100).WithMessage("Page size must be between 1 and 100.");
    }
}