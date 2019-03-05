using FluentValidation;
using VideoApi.Contract.Requests;

namespace Video.API.Validations
{
    public class UpdateParticipantStatusRequestValidation : AbstractValidator<UpdateParticipantStatusRequest>
    {
        public static readonly string InvalidStateErrorMessage = "Invalid state provided";
        
        public UpdateParticipantStatusRequestValidation()
        {
            RuleFor(x => x.State).IsInEnum().WithMessage(InvalidStateErrorMessage);
        }
    }
}