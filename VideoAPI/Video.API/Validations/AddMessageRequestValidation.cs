using FluentValidation;
using VideoApi.Contract.Requests;

namespace Video.API.Validations
{
    public class AddMessageRequestValidation : AbstractValidator<AddMessageRequest>
    {
        public static readonly string NoFromErrorMessage = "From is required";
        public static readonly string NoToErrorMessage = "To is required";
        public static readonly string NoMessageTextErrorMessage = "MessageText is required";

        public AddMessageRequestValidation()
        {
            RuleFor(x => x.From).NotEmpty().WithMessage(NoFromErrorMessage);
            RuleFor(x => x.To).NotEmpty().WithMessage(NoToErrorMessage);
            RuleFor(x => x.MessageText).NotEmpty().WithMessage(NoMessageTextErrorMessage);
        }
    }
}
