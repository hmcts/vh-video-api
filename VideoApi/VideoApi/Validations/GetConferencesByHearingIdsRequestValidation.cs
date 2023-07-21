using FluentValidation;
using VideoApi.Contract.Requests;

namespace VideoApi.Validations
{
    public class GetConferencesByHearingIdsRequestValidation : AbstractValidator<GetConferencesByHearingIdsRequest>
    {
        public const string HearingIdNotSpecifiedError = "Hearing venue must be specified";
        public GetConferencesByHearingIdsRequestValidation()
        {
            RuleFor(x => x.HearingRefIds).NotEmpty().WithMessage(HearingIdNotSpecifiedError);
        }
    }
}
