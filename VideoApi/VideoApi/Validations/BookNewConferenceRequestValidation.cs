using System;
using FluentValidation;
using VideoApi.Contract.Requests;

namespace VideoApi.Validations
{
    public class BookNewConferenceRequestValidation : AbstractValidator<BookNewConferenceRequest>
    {
        public static readonly string NoHearingRefIdErrorMessage = "HearingRefId is required";
        public static readonly string NoCaseTypeErrorMessage = "CaseType is required";
        public static readonly string ScheduleDateTimeInPastErrorMessage = "ScheduledDateTime cannot be in the past";
        public static readonly string ScheduledDurationErrorMessage = "ScheduledDuration is required";
        public static readonly string NoCaseNumberErrorMessage = "CaseNumber is required";
        public static readonly string NoParticipantsErrorMessage = "Please provide at least one participant";
        public static readonly string NoCaseTypeServiceIdErrorMessage = "CaseTypeServiceId is required";
        
        public BookNewConferenceRequestValidation()
        {
            RuleFor(x => x.HearingRefId).NotEmpty().WithMessage(NoHearingRefIdErrorMessage);
            RuleFor(x => x.CaseType).NotEmpty().WithMessage(NoCaseTypeErrorMessage);
            RuleFor(x => x.CaseNumber).NotEmpty().WithMessage(NoCaseNumberErrorMessage);
            RuleFor(x => x.ScheduledDuration).NotEmpty().WithMessage(ScheduledDurationErrorMessage);
            RuleFor(x => x.ScheduledDateTime.Date)
                .GreaterThanOrEqualTo(DateTime.Now.Date).WithMessage(ScheduleDateTimeInPastErrorMessage);
            RuleFor(x => x.CaseTypeServiceId).NotEmpty().WithMessage(NoCaseTypeServiceIdErrorMessage);
            
            
            RuleFor(x => x.Participants).NotEmpty()
                .NotEmpty().WithMessage(NoParticipantsErrorMessage);
            
            RuleForEach(x => x.Participants)
                .SetValidator(new ParticipantRequestValidation());
            
            RuleForEach(x=> x.Endpoints)
                .SetValidator(new AddEndpointRequestValidation());
        }
    }
}
