using FluentValidation;
using VideoApi.Contract.Requests;

namespace Video.API.Validations
{
    public class UpdateParticipantRequestValidation : AbstractValidator<UpdateParticipantRequest>
    {
        public static readonly string NoNameErrorMessage = "Fullname is required";
        public static readonly string NoDisplayNameErrorMessage = "DisplayName is required";

        public UpdateParticipantRequestValidation()
        {
            RuleFor(x => x.Fullname).NotEmpty().WithMessage(NoNameErrorMessage);
            RuleFor(x => x.DisplayName).NotEmpty().WithMessage(NoDisplayNameErrorMessage);
        }

    }
}