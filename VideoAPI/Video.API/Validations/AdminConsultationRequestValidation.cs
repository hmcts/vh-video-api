using FluentValidation;
using VideoApi.Contract.Requests;
using VideoApi.Domain.Enums;

namespace Video.API.Validations
{
    public class AdminConsultationRequestValidation : AbstractValidator<AdminConsultationRequest>
    {
        public static readonly string NoConferenceIdErrorMessage = "ConferenceId is required";
        public static readonly string NoParticipantIdErrorMessage = "ParticipantId is required";
        public static readonly string NoAnswerErrorMessage = "Answer to request is required";
        public static readonly string NotValidConsultationRoomMessage = "Room must be a consultation room";

        public AdminConsultationRequestValidation()
        {
            RuleFor(x => x.ConferenceId).NotEmpty().WithMessage(NoConferenceIdErrorMessage);
            RuleFor(x => x.ParticipantId).NotEmpty().WithMessage(NoParticipantIdErrorMessage);
            RuleFor(x => x.Answer).NotEqual(ConsultationAnswer.None).WithMessage(NoAnswerErrorMessage);
            RuleFor(x => x.ConsultationRoom).Custom((type, context) =>
            {
                if (type != RoomType.ConsultationRoom)
                {
                    context.AddFailure("ConsultationRoom", NotValidConsultationRoomMessage);
                }
            });
        }
    }
}
