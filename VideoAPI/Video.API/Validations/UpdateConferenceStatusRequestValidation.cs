using FluentValidation;
using VideoApi.Contract.Requests;

namespace Video.API.Validations
{
    public class UpdateConferenceStatusRequestValidation : AbstractValidator<UpdateConferenceStatusRequest>
    {
        public static readonly string InvalidStateErrorMessage = "Invalid state provided";
        
        public UpdateConferenceStatusRequestValidation()
        {
            RuleFor(x => x.State).IsInEnum().WithMessage(InvalidStateErrorMessage);
        }
    }
}