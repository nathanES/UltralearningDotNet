using FluentValidation;
using Task = TaskManagement.Common.Models.Task;

namespace TaskManagement.Tasks.Validators;

public class TaskValidator : AbstractValidator<Task>
{
    public TaskValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty();
    }
}