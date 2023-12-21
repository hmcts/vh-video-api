using FluentValidation;
using VideoApi.Contract.Requests;

namespace VideoApi.Validations
{
    public class UpdateParticipantUsernameRequestValidation : AbstractValidator<UpdateParticipantUsernameRequest>
    {
        public static readonly string NoUsernameErrorMessage = "Username is required";

        public UpdateParticipantUsernameRequestValidation()
        {
            RuleFor(x => x.Username).NotEmpty().WithMessage(NoUsernameErrorMessage);
        }
    }
}
