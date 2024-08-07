using System;
using FluentValidation;
using VideoApi.Contract.Requests;

namespace VideoApi.Validations
{
    public class UpdateConferenceRequestValidation : AbstractValidator<UpdateConferenceRequest>
    {
        public static readonly string NoHearingRefIdErrorMessage = "HearingRefId is required";
        public static readonly string ScheduleDateTimeInPastErrorMessage = "ScheduledDateTime cannot be in the past";
        public static readonly string ScheduledDurationErrorMessage = "ScheduledDuration is required";
        
        public UpdateConferenceRequestValidation()
        {
            RuleFor(x => x.HearingRefId).NotEmpty().WithMessage(NoHearingRefIdErrorMessage);
            RuleFor(x => x.ScheduledDuration).NotEmpty().WithMessage(ScheduledDurationErrorMessage);
            RuleFor(x => x.ScheduledDateTime.Date)
                .GreaterThanOrEqualTo(DateTime.Now.Date).WithMessage(ScheduleDateTimeInPastErrorMessage);
        }
    }
}
