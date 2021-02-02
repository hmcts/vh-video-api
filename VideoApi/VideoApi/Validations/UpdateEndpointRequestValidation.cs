using FluentValidation;
using VideoApi.Contract.Requests;

namespace VideoApi.Validations
{
    public class UpdateEndpointRequestValidation : AbstractValidator<UpdateEndpointRequest>
    {
        public const string NoDisplayNameError = "DisplayName is required";
        public UpdateEndpointRequestValidation()
        {
            RuleFor(x => x.DisplayName).NotEmpty().WithMessage(NoDisplayNameError);
        }
    }
}
