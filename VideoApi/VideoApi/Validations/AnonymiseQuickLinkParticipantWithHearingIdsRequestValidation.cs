using FluentValidation;
using VideoApi.Contract.Requests;

namespace VideoApi.Validations
{
    public class AnonymiseQuickLinkParticipantWithHearingIdsRequestValidation : AbstractValidator<AnonymiseQuickLinkParticipantWithHearingIdsRequest>
    {
        public const string NoHearingIdsErrorMessage =
            "No hearing ids were passed for the anonymisation request for quick link participants";

        public AnonymiseQuickLinkParticipantWithHearingIdsRequestValidation()
        {
            RuleFor(x => x.HearingIds).NotEmpty().WithMessage(NoHearingIdsErrorMessage);
        }
    }
}
