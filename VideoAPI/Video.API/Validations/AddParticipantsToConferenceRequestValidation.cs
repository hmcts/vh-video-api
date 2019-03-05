using FluentValidation;
using VideoApi.Contract.Requests;

namespace Video.API.Validations
{
    public class AddParticipantsToConferenceRequestValidation : AbstractValidator<AddParticipantsToConferenceRequest>
    {
        public static readonly string NoParticipantsErrorMessage = "Please provide at least one participant";
        
        public AddParticipantsToConferenceRequestValidation()
        {
            RuleFor(x => x.Participants).NotEmpty()
                .NotEmpty().WithMessage(NoParticipantsErrorMessage);
            
            RuleForEach(x => x.Participants)
                .SetValidator(new ParticipantRequestValidation());
        }
    }
}