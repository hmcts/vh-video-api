using FluentValidation;
using VideoApi.Contract.Requests;

namespace VideoApi.Validations
{
    public class UpdateParticipantRequestValidation : AbstractValidator<UpdateParticipantRequest>
    {
        public const string NoDisplayNameErrorMessage = "DisplayName is required";
        public const string InvalidDisplayNameErrorMessage = "DisplayName is invalid, does not match regex";
        private const string NameRegex = @"^[\p{L}\p{N}\s',._-]+${1,255}$";
        
        public UpdateParticipantRequestValidation()
        {
            RuleFor(x => x.DisplayName)
                .NotEmpty().WithMessage(NoDisplayNameErrorMessage)
                .Matches(NameRegex).WithMessage(InvalidDisplayNameErrorMessage);
        }
    }
}
