using FluentValidation;
using VideoApi.Contract.Requests;

namespace Video.API.Validations
{
    public class LeaveConsultationRequestValidation : AbstractValidator<LeaveConsultationRequest>
    {
        public static readonly string NoConferenceIdErrorMessage =
            "ConferenceId is required";
        
        public static readonly string NoParticipantIdErrorMessage =
            "ParticipantId is required";

        public LeaveConsultationRequestValidation()
        {
            RuleFor(x => x.ConferenceId).NotEmpty().WithMessage(NoConferenceIdErrorMessage);
            RuleFor(x => x.ParticipantId).NotEmpty().WithMessage(NoParticipantIdErrorMessage);
        }
    }
}