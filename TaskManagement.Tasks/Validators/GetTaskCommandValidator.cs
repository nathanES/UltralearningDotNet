using FluentValidation;
using TaskManagement.Tasks.Commands.GetTask;

namespace TaskManagement.Tasks.Validators;

public class GetTaskCommandValidator : AbstractValidator<GetTaskCommand>
{
    public GetTaskCommandValidator()
    {
        RuleFor(x => x)
            .NotNull();
        RuleFor(x => x.Id)
            .NotEmpty();
    }
}