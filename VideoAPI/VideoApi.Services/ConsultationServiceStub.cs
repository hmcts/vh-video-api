using System;
using System.Diagnostics.CodeAnalysis;
using VideoApi.Domain.Enums;
using VideoApi.Services.Contracts;
using Task = System.Threading.Tasks.Task;

namespace VideoApi.Services
{
    [ExcludeFromCodeCoverage]
    public class ConsultationServiceStub : IConsultationService
    {

        public Task TransferParticipantAsync(Guid conferenceId, Guid participantId, VirtualCourtRoomType fromRoom,
            VirtualCourtRoomType toRoom)
        {
            return Task.CompletedTask;
        }

        public Task LeaveConsultationAsync(Guid conferenceId, Guid participantId, VirtualCourtRoomType consultation)
        {
            return Task.CompletedTask;
        }
    }
}
