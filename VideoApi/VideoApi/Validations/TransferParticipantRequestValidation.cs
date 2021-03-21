using System;
using FluentValidation;
using VideoApi.Contract.Requests;

namespace VideoApi.Validations
{
    public class TransferParticipantRequestValidation : AbstractValidator<TransferParticipantRequest>
    {
        public const string MissingParticipantId = "ParticipantId is required";
        public const string MissingTransferType = "TransferType is required";

        public TransferParticipantRequestValidation()
        {
            RuleFor(x => x.ParticipantId).NotEqual(Guid.Empty).WithMessage(MissingParticipantId);
            RuleFor(x => x.TransferType).NotEmpty().WithMessage(MissingTransferType).WithMessage(MissingTransferType);
        }
    }
}
