using FluentValidation;
using TaskManagement.Common.Models;
using TaskManagement.Tasks.Commands.UpdateTask;
using TaskManagement.Tasks.Models;

namespace TaskManagement.Tasks.Validators;

public class UpdateTaskCommandValidator : AbstractValidator<UpdateTaskCommand>
{
    public UpdateTaskCommandValidator()
    {
        RuleFor(x => x)
            .NotNull();
        RuleFor(x => x.Id)
            .NotEmpty();
        RuleFor(x => x.Title)
            .NotNull()
            .NotEmpty()
            .MaximumLength(255).WithMessage("Title cannot be longer than 255 characters.");
        
        RuleFor(x => x.Priority)
            .Must(priority => priority == null || Enum.IsDefined(typeof(Priority), priority))
            .WithMessage("Invalid priority value. If provided, it must be a valid enum value.");
        
        RuleFor(x => x.Deadline)
            .Must((command, deadline) => command.Priority == Priority.High || deadline.HasValue)
            .WithMessage("Deadline is required for high-priority tasks.");

    }
}