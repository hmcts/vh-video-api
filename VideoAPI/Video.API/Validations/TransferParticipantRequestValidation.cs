using FluentValidation;
using VideoApi.Contract.Requests;

namespace Video.API.Validations
{
    public class TransferParticipantRequestValidation : AbstractValidator<TransferParticipantRequest>
    {
        public const string MissingParticipantId = "ParticipantId is required";
        public const string MissingTransferType = "TransferType is required";

        public TransferParticipantRequestValidation()
        {
            RuleFor(x => x.ParticipantId).NotEmpty().WithMessage(MissingParticipantId);
            RuleFor(x => x.TransferType).NotEmpty().WithMessage(MissingTransferType).WithMessage(MissingTransferType);
        }
    }
}
