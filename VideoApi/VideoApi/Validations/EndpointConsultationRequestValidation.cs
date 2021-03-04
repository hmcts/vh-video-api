using FluentValidation;
using VideoApi.Contract.Requests;

namespace VideoApi.Validations
{
    public class EndpointConsultationRequestValidation : AbstractValidator<EndpointConsultationRequest>
    {
        public const string NoConferenceError = "ConferenceId is required";
        public const string NoEndpointError = "EndpointId is required";
        public EndpointConsultationRequestValidation()
        { 
            RuleFor(x => x.ConferenceId).NotEmpty().WithMessage(NoConferenceError);
            RuleFor(x => x.EndpointId).NotEmpty().WithMessage(NoEndpointError);
        }
    }
}
