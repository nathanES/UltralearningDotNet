using FluentValidation;

namespace TaskManagement.Tasks.Validators;

public class TaskValidator : AbstractValidator<Task>
{
    public TaskValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty();
    }
}