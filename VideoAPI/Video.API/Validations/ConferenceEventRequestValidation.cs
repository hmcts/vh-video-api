using FluentValidation;
using VideoApi.Contract.Requests;
using VideoApi.Domain.Enums;

namespace Video.API.Validations
{
    public class ConferenceEventRequestValidation : AbstractValidator<ConferenceEventRequest>
    {
        public static readonly string NoConferenceIdErrorMessage = "ConferenceId is required";
        public static readonly string NoEventIdErrorMessage = "EventId is required";
        public static readonly string NoParticipantIdErrorMessage = "ParticipantId is required";
        public static readonly string NoEventTypeErrorMessage = "EventType is required";
        public static readonly string NoTransferFromErrorMessage = "Room type for 'TransferredFrom' is not recognised";
        public static readonly string NoTransferToErrorMessage = "Room type for 'TransferredTo' is not recognised";

        public ConferenceEventRequestValidation()
        {
            RuleFor(x => x.ConferenceId).NotEmpty().WithMessage(NoConferenceIdErrorMessage);
            RuleFor(x => x.EventId).NotEmpty().WithMessage(NoEventIdErrorMessage);
            RuleFor(x => x.ParticipantId).NotEmpty().WithMessage(NoParticipantIdErrorMessage);
            RuleFor(x => x.EventType)
                .IsInEnum().WithMessage(NoEventTypeErrorMessage)
                .NotEqual(EventType.None).WithMessage(NoEventTypeErrorMessage);
            RuleFor(x => x.TransferFrom).IsInEnum().WithMessage(NoTransferFromErrorMessage);
            RuleFor(x => x.TransferTo).IsInEnum().WithMessage(NoTransferToErrorMessage);
        }
    }
}