using System;
using FluentValidation;
using VideoApi.Contract.Requests;

namespace VideoApi.Validations
{
    public class TransferParticipantRequestValidation : AbstractValidator<TransferParticipantRequest>
    {
        public const string MissingParticipantId = "ParticipantId or RoomId is required";
        public const string MissingTransferType = "TransferType is required";

        public TransferParticipantRequestValidation()
        {
            RuleFor(x => x.ParticipantId)
                .NotEqual(Guid.Empty).WithMessage(MissingParticipantId)
                .NotEmpty().When(x => !x.RoomId.HasValue).WithMessage(MissingParticipantId);
            RuleFor(x => x.RoomId)
                .GreaterThan(0).When(x => !x.ParticipantId.HasValue).WithMessage(MissingParticipantId);
            RuleFor(x => x.TransferType).NotEmpty().WithMessage(MissingTransferType).WithMessage(MissingTransferType);
        }
    }
}
