using FluentValidation;
using VideoApi.Contract.Requests;

namespace VideoApi.Validations
{
    public class AddMagicLinkParticipantRequestValidation : AbstractValidator<AddMagicLinkParticipantRequest>
    {
        public static readonly string NoNameErrorMessage = "Name is required";
        public static readonly string NoUserRoleErrorMessage = "UserRole is required";

        public AddMagicLinkParticipantRequestValidation()
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage(NoNameErrorMessage);
            RuleFor(x => x.UserRole).NotEmpty().WithMessage(NoUserRoleErrorMessage);
        }
    }
}
