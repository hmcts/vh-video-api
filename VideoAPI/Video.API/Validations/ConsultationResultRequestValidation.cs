using FluentValidation;
using VideoApi.Contract.Requests;

namespace Video.API.Validations
{
    public class ConsultationResultRequestValidation : AbstractValidator<ConsultationResultRequest>
    {
        public static readonly string NoConferenceIdErrorMessage = "ConferenceId is required";
        public static readonly string NoRequestedForIdErrorMessage = "RequestedFor is required";
        public static readonly string NoRequestedByIdErrorMessage = "RequestedBy is required";
        public static readonly string NoAnswerErrorMessage = "Answer to request is required";

        public ConsultationResultRequestValidation()
        {
            RuleFor(x => x.ConferenceId).NotEmpty().WithMessage(NoConferenceIdErrorMessage);
            RuleFor(x => x.RequestedBy).NotEmpty().WithMessage(NoRequestedByIdErrorMessage);
            RuleFor(x => x.RequestedFor).NotEmpty().WithMessage(NoRequestedForIdErrorMessage);
            RuleFor(x => x.Answer).NotEqual(ConsultationRequestAnswer.None).WithMessage(NoAnswerErrorMessage);
        }
    }
}