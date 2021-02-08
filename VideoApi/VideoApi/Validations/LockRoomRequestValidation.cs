using FluentValidation;
using VideoApi.Contract.Requests;

namespace Video.API.Validations
{
    public class LockRoomRequestValidation : AbstractValidator<LockRoomRequest>
    {
        public const string NoConferenceError = "ConferenceId is required";
        public const string NoRoomLabelError = "RoomLabel is required";
        public LockRoomRequestValidation()
        {
            RuleFor(x => x.ConferenceId).NotEmpty().WithMessage(NoConferenceError);
            RuleFor(x => x.RoomLabel).NotEmpty().WithMessage(NoRoomLabelError);
        }
    }
}
