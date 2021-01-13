using System;
using System.Diagnostics.CodeAnalysis;
using VideoApi.Services.Contracts;
using Task = System.Threading.Tasks.Task;

namespace VideoApi.Services
{
    [ExcludeFromCodeCoverage]
    public class ConsultationServiceStub : IConsultationService
    {

        public Task TransferParticipantAsync(Guid conferenceId, Guid participantId, string fromRoom,
            string toRoom)
        {
            return Task.CompletedTask;
        }

        public Task LeaveConsultationAsync(Guid conferenceId, Guid participantId, string room)
        {
            return Task.CompletedTask;
        }
    }
}
