using FluentValidation;
using VideoApi.Contract.Requests;

namespace VideoApi.Validations
{
    public class ConsultationRequestValidation : AbstractValidator<ConsultationRequestResponse>
    {
        public static readonly string NoConferenceIdErrorMessage = "ConferenceId is required";
        public static readonly string NoRequestedForIdErrorMessage = "RequestedFor is required";
        
        public ConsultationRequestValidation()
        {
            RuleFor(x => x.ConferenceId).NotEmpty().WithMessage(NoConferenceIdErrorMessage);
            RuleFor(x => x.RequestedFor).NotEmpty().WithMessage(NoRequestedForIdErrorMessage);
        }
    }
}
