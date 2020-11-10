using FluentValidation;
using VideoApi.Contract.Requests;

namespace Video.API.Validations
{
    public class AddTaskRequestValidation : AbstractValidator<AddTaskRequest>
    {
        public const string NoParticipantError = "ParticipantId is required";
        public const string NoBodyError = "Body is required";
        public const string NoTaskTypeError = "Task type is required";

        public AddTaskRequestValidation()
        {
            RuleFor(x => x.ParticipantId).NotEmpty().WithMessage(NoParticipantError);
            RuleFor(x => x.Body).NotEmpty().WithMessage(NoBodyError);
            RuleFor(x => x.TaskType).IsInEnum().WithMessage(NoTaskTypeError);
        }
    }
}
