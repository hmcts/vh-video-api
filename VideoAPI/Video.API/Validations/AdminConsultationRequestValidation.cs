using FluentValidation;
using VideoApi.Contract.Requests;

namespace Video.API.Validations
{
    public class ConsultationRequestResponseValidation : AbstractValidator<ConsultationRequestResponse>
    {
        public static readonly string NoConferenceIdErrorMessage = "ConferenceId is required";
        public static readonly string NoParticipantIdErrorMessage = "ParticipantId is required";
        public static readonly string NoAnswerErrorMessage = "Answer to request is required";
        public static readonly string NotValidConsultationRoomMessage = "Room must be a consultation room";

        public ConsultationRequestResponseValidation()
        {
            RuleFor(x => x.ConferenceId).NotEmpty().WithMessage(NoConferenceIdErrorMessage);
            RuleFor(x => x.RequestedFor).NotEmpty().WithMessage(NoParticipantIdErrorMessage);
            RuleFor(x => x.Answer).NotEqual(ConsultationAnswer.None).WithMessage(NoAnswerErrorMessage);
        }
    }
}
