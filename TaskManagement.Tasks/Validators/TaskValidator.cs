using FluentValidation;

namespace TaskManagement.Tasks.Validators;

public class TaskValidator : AbstractValidator<Models.Task>
{
    public TaskValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty(); 
    }
}