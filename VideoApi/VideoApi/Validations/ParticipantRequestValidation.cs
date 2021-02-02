using FluentValidation;
using VideoApi.Contract.Requests;

namespace VideoApi.Validations
{
    public class ParticipantRequestValidation : AbstractValidator<ParticipantRequest>
    {
        public static readonly string NoParticipantRefIdErrorMessage = "ParticipantRefId is required";
        public static readonly string NoNameErrorMessage = "Name is required";
        public static readonly string NoDisplayNameErrorMessage = "DisplayName is required";
        public static readonly string NoUsernameErrorMessage = "Username is required";
        public static readonly string NoUserRoleErrorMessage = "UserRole is required";
        public static readonly string NoCaseTypeGroupErrorMessage = "CaseTypeGroup is required";
        public static readonly string NoHearingRoleErrorMessage = "HearingRole is required";
        public static readonly string NoFirstNameErrorMessage = "FistName is required";
        public static readonly string NoLastNameErrorMessage = "LastName is required";
        public static readonly string NoContactEmailErrorMessage = "ContactEmail is required";

        public ParticipantRequestValidation()
        {
            RuleFor(x => x.ParticipantRefId).NotEmpty().WithMessage(NoParticipantRefIdErrorMessage);
            RuleFor(x => x.Name).NotEmpty().WithMessage(NoNameErrorMessage);
            RuleFor(x => x.FirstName).NotEmpty().WithMessage(NoFirstNameErrorMessage);
            RuleFor(x => x.LastName).NotEmpty().WithMessage(NoLastNameErrorMessage);
            RuleFor(x => x.DisplayName).NotEmpty().WithMessage(NoDisplayNameErrorMessage);
            RuleFor(x => x.Username).NotEmpty().WithMessage(NoUsernameErrorMessage);
            RuleFor(x => x.UserRole).NotEmpty().WithMessage(NoUserRoleErrorMessage);
            RuleFor(x => x.HearingRole).NotEmpty().WithMessage(NoHearingRoleErrorMessage);
            RuleFor(x => x.CaseTypeGroup).NotEmpty().WithMessage(NoCaseTypeGroupErrorMessage);
            RuleFor(x => x.ContactEmail).NotEmpty().WithMessage(NoContactEmailErrorMessage);
        }
    }
}
