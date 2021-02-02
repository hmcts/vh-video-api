using System;
using FluentValidation;
using VideoApi.Contract.Requests;
using VideoApi.Domain.Enums;

namespace VideoApi.Validations
{
    public class ConferenceEventRequestValidation : AbstractValidator<ConferenceEventRequest>
    {
        public static readonly string NoConferenceIdErrorMessage = "ConferenceId is required";
        public static readonly string InvalidConferenceIdFormatErrorMessage = "ConferenceId format is not recognised";
        public static readonly string NoEventIdErrorMessage = "EventId is required";
        public static readonly string NoParticipantIdErrorMessage = "ParticipantId is required";
        public static readonly string InvalidParticipantIdFormatErrorMessage = "ParticipantId format is not recognised";
        public static readonly string NoEventTypeErrorMessage = "EventType is required";

        public ConferenceEventRequestValidation()
        {
            RuleFor(x => x.ConferenceId).NotEmpty().WithMessage(NoConferenceIdErrorMessage);
            RuleFor(x => x.ConferenceId).Must(x => Guid.TryParse(x, out _))
                .WithMessage(InvalidConferenceIdFormatErrorMessage);
            
            RuleFor(x => x.EventId).NotEmpty().WithMessage(NoEventIdErrorMessage);
            RuleFor(x => x.ParticipantId).NotEmpty().When(x => x.EventType != EventType.Suspend).WithMessage(NoParticipantIdErrorMessage);
            RuleFor(x => x.ParticipantId).Must(x => Guid.TryParse(x, out _)).When(x => x.EventType != EventType.Suspend)
                .WithMessage(InvalidParticipantIdFormatErrorMessage);
            
            RuleFor(x => x.EventType)
                .IsInEnum().WithMessage(NoEventTypeErrorMessage)
                .NotEqual(EventType.None).WithMessage(NoEventTypeErrorMessage);
        }
    }
}
