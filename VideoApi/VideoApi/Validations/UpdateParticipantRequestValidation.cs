using FluentValidation;
using VideoApi.Contract.Requests;

namespace VideoApi.Validations
{
    public class UpdateParticipantRequestValidation : AbstractValidator<UpdateParticipantRequest>
    {
        public static readonly string NoDisplayNameErrorMessage = "DisplayName is required";

        public UpdateParticipantRequestValidation()
        {
            RuleFor(x => x.DisplayName).NotEmpty().WithMessage(NoDisplayNameErrorMessage);
        }

    }
}
