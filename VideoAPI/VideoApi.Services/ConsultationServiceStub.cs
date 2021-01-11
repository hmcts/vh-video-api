using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using VideoApi.Domain.Enums;
using VideoApi.Services.Contracts;

namespace VideoApi.Services
{
    [ExcludeFromCodeCoverage]
    public class ConsultationServiceStub : IConsultationService
    {
        public Task StartConsultationAsync(Guid conferenceId, Guid requestedBy, VirtualCourtRoomType roomType)
        {
            return Task.CompletedTask;
        }

        public Task TransferParticipantAsync(Guid conferenceId, Guid participantId, VirtualCourtRoomType fromRoom,
            VirtualCourtRoomType toRoom)
        {
            return Task.CompletedTask;
        }
    }
}
