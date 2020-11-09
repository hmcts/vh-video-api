using FluentValidation;
using VideoApi.Contract.Requests;

namespace Video.API.Validations
{
    public class AddTaskRequestValidation : AbstractValidator<AddTaskRequest>
    {
        public const string NoTaskTypeError = "Task type is required";

        public AddTaskRequestValidation()
        {
            RuleFor(x => x.TaskType).IsInEnum().WithMessage(NoTaskTypeError);
        }
    }
}
