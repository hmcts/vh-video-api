using FluentValidation;
using VideoApi.Contract.Requests;

namespace VideoApi.Validations
{
    public class StartHearingRequestValidation : AbstractValidator<StartHearingRequest>
    {
        public const string LayoutError = "DisplayName is required";

        public StartHearingRequestValidation()
        {
            RuleFor(x => x.Layout.Value).IsInEnum().WithMessage(LayoutError);
        }
    }
}
