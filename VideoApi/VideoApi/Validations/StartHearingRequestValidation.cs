using FluentValidation;
using VideoApi.Contract.Requests;

namespace Video.API.Validations
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
