using FluentValidation;
using VideoApi.Contract.Requests;
using VideoApi.Domain.Enums;

namespace Video.API.Validations
{
    public class UpdateConferenceStatusRequestValidation : AbstractValidator<UpdateConferenceStatusRequest>
    {
        public static readonly string InvalidStateErrorMessage = "Invalid state provided";
        
        public UpdateConferenceStatusRequestValidation()
        {
            RuleFor(x => x.State).IsInEnum().WithMessage(InvalidStateErrorMessage)
                .NotEqual(ConferenceState.None).WithMessage(InvalidStateErrorMessage);
        }
    }
}