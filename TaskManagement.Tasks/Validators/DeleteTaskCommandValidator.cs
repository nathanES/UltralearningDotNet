using FluentValidation;
using TaskManagement.Tasks.Commands.DeleteTask;

namespace TaskManagement.Tasks.Validators;

public class DeleteTaskCommandValidator : AbstractValidator<DeleteTaskCommand>
{
    public DeleteTaskCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty();
    }
}