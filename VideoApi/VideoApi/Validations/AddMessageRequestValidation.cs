using FluentValidation;
using VideoApi.Contract.Requests;

namespace Video.API.Validations
{
    public class AddMessageRequestValidation : AbstractValidator<AddInstantMessageRequest>
    {
        public static readonly string NoFromErrorMessage = "From is required";
        public static readonly string NoMessageTextErrorMessage = "MessageText is required";
        private static readonly int MaxCharLength = 256;
        public static readonly string MaxMessageLength = $"MessageText cannot exceed {MaxCharLength} characters";
        public static readonly string NoToErrorMessage = "To is required";

        public AddMessageRequestValidation()
        {
            RuleFor(x => x.From).NotEmpty().WithMessage(NoFromErrorMessage);
            RuleFor(x => x.MessageText)
                .NotEmpty().WithMessage(NoMessageTextErrorMessage)
                .MaximumLength(MaxCharLength).WithMessage(MaxMessageLength);
            RuleFor(x => x.To).NotEmpty().WithMessage(NoToErrorMessage);
        }
    }
}
