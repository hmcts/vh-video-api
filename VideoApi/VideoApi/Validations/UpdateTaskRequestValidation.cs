using FluentValidation;
using VideoApi.Contract.Requests;

namespace VideoApi.Validations
{
    public class UpdateTaskRequestValidation : AbstractValidator<UpdateTaskRequest>
    {
        public static readonly string NoUsernameErrorMessage =
            "Please provide the username of the person updating the task";

        public UpdateTaskRequestValidation()
        {
            RuleFor(x => x.UpdatedBy).NotEmpty().WithMessage(NoUsernameErrorMessage);
        }
    }
}
