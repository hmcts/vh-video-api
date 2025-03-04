using FluentValidation;
using VideoApi.Contract.Requests;

namespace VideoApi.Validations
{
    public class ParticipantRequestValidation : AbstractValidator<ParticipantRequest>
    {
        public const string NoParticipantRefIdErrorMessage = "ParticipantRefId is required";
        public const string NoDisplayNameErrorMessage = "DisplayName is required";
        public const string NoUsernameErrorMessage = "Username is required";
        public const string NoUserRoleErrorMessage = "UserRole is required";
        public const string NoHearingRoleErrorMessage = "HearingRole is required";
        public const string NoContactEmailErrorMessage = "ContactEmail is required";

        public ParticipantRequestValidation()
        {
            RuleFor(x => x.ParticipantRefId).NotEmpty().WithMessage(NoParticipantRefIdErrorMessage);
            RuleFor(x => x.DisplayName).NotEmpty().WithMessage(NoDisplayNameErrorMessage);
            RuleFor(x => x.Username).NotEmpty().WithMessage(NoUsernameErrorMessage);
            RuleFor(x => x.UserRole).NotEmpty().WithMessage(NoUserRoleErrorMessage);
            RuleFor(x => x.HearingRole).NotEmpty().WithMessage(NoHearingRoleErrorMessage);
            RuleFor(x => x.ContactEmail).NotEmpty().WithMessage(NoContactEmailErrorMessage);
        }
    }
}
