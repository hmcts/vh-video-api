using FluentValidation;
using VideoApi.Contract.Requests;

namespace VideoApi.Validations
{
    public class AddEndpointRequestValidation : AbstractValidator<AddEndpointRequest>
    {
        public const string NoDisplayNameError = "DisplayName is required";
        public const string NoSipError = "SipAddress is required";
        public const string NoPinError = "Pin is required";

        public AddEndpointRequestValidation()
        {
            RuleFor(x => x.DisplayName).NotEmpty().WithMessage(NoDisplayNameError);
            RuleFor(x => x.SipAddress).NotEmpty().WithMessage(NoSipError);
            RuleFor(x => x.Pin).NotEmpty().WithMessage(NoPinError);
        }
    }
}
