using FluentValidation;
using VideoApi.Contract.Requests;

namespace VideoApi.Validations
{
    public class AnonymiseConferenceWithHearingIdsRequestValidation: AbstractValidator<AnonymiseConferenceWithHearingIdsRequest>
    {
        public const string NoHearingIdsErrorMessage =
            "No hearing ids were passed for the anonymisation request for quick link participants";

        public AnonymiseConferenceWithHearingIdsRequestValidation()
        {
            RuleFor(x => x.HearingIds).NotEmpty().WithMessage(NoHearingIdsErrorMessage);
        }
    }
}
