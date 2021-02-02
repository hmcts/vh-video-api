using FluentValidation;
using VideoApi.Contract.Requests;

namespace VideoApi.Validations
{
    public class UpdateParticipantRequestValidation : AbstractValidator<UpdateParticipantRequest>
    {
        public static readonly string NoNameErrorMessage = "Fullname is required";
        public static readonly string NoFirstNameErrorMessage = "FirstName is required";
        public static readonly string NoLastNameErrorMessage = "LastName is required";
        public static readonly string NoDisplayNameErrorMessage = "DisplayName is required";

        public UpdateParticipantRequestValidation()
        {
            RuleFor(x => x.Fullname).NotEmpty().WithMessage(NoNameErrorMessage);
            RuleFor(x => x.FirstName).NotEmpty().WithMessage(NoFirstNameErrorMessage);
            RuleFor(x => x.LastName).NotEmpty().WithMessage(NoLastNameErrorMessage);
            RuleFor(x => x.DisplayName).NotEmpty().WithMessage(NoDisplayNameErrorMessage);
        }

    }
}
