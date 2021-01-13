using FluentValidation;
using VideoApi.Contract.Requests;

namespace Video.API.Validations
{
    public class StartConsultationRequestValidation: AbstractValidator<StartConsultationRequest>
    {
        public StartConsultationRequestValidation()
        {
            RuleFor(x => x.ConferenceId).NotEmpty();
            RuleFor(x => x.RequestedBy).NotEmpty();
            RuleFor(x => x.RoomType).IsInEnum();
        }
    }
}
