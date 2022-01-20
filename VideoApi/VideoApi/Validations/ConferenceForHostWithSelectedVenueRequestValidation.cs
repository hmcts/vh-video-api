using FluentValidation;
using VideoApi.Contract.Requests;

namespace VideoApi.Validations
{
    public class ConferenceForHostWithSelectedVenueRequestValidation : AbstractValidator<ConferenceForStaffMembertWithSelectedVenueRequest>
    {
        public const string HearingVenueNotSpecifiedError = "Hearing venue must be specified";
        public ConferenceForHostWithSelectedVenueRequestValidation()
        {
            RuleFor(x => x.HearingVenueNames).NotEmpty().WithMessage(HearingVenueNotSpecifiedError);
        }
    }
}
