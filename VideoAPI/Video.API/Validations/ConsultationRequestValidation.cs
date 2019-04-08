using System;
using FluentValidation;
using VideoApi.Contract.Requests;

namespace Video.API.Validations
{
    public class ConsultationRequestValidation : AbstractValidator<ConsultationRequest>
    {
        public static readonly string NoConferenceIdErrorMessage = "ConferenceId is required";
        public static readonly string NoRequestedForIdErrorMessage = "RequestedFor is required";
        public static readonly string NoRequestedByIdErrorMessage = "RequestedBy is required";

        public ConsultationRequestValidation()
        {
            RuleFor(x => x.ConferenceId).NotEqual(Guid.Empty).WithMessage(NoConferenceIdErrorMessage);
            RuleFor(x => x.RequestedBy).NotEqual(Guid.Empty).WithMessage(NoRequestedByIdErrorMessage);
            RuleFor(x => x.RequestedFor).NotEqual(Guid.Empty).WithMessage(NoRequestedForIdErrorMessage);
        }
    }
}