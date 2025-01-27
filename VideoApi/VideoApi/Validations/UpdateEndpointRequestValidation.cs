using FluentValidation;
using VideoApi.Contract.Requests;

namespace VideoApi.Validations
{
    public class UpdateEndpointRequestValidation : AbstractValidator<UpdateEndpointRequest>
    {
        public const string NoDisplayNameError = "DisplayName is required";
        public const string InvalidRoleError = "Coference role must be 'Host' or 'Guest'";
        
        public UpdateEndpointRequestValidation()
        {
            RuleFor(x => x.DisplayName).NotEmpty().WithMessage(NoDisplayNameError);
            RuleFor(x=> x.ConferenceRole).IsInEnum();
            RuleFor(x=> x.ConferenceRole).IsInEnum().WithMessage(InvalidRoleError);
        }
    }
}
