using FluentValidation;
using VideoApi.Contract.Requests;

namespace VideoApi.Validations
{
    public class UpdateConferenceParticipantsRequestValidation : AbstractValidator<UpdateConferenceParticipantsRequest>
    {
        public static readonly string NoParticipantsErrorMessage = "At least one participant is required";
        
        public UpdateConferenceParticipantsRequestValidation()
        {
            RuleFor(x => x.NewParticipants.Count + x.ExistingParticipants.Count + x.RemovedParticipants.Count + x.LinkedParticipants.Count)
                .NotEmpty().WithMessage(NoParticipantsErrorMessage);
        }
    }
}
