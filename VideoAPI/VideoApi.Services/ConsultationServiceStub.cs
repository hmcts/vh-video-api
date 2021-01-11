using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using VideoApi.Domain.Enums;
using VideoApi.Services.Contracts;
using VideoApi.Services.Kinly;

namespace VideoApi.Services
{
    [ExcludeFromCodeCoverage]
    public class ConsultationServiceStub : IConsultationService
    {
        public Task<CreateConsultationRoomResponse> CreateConsultationRoomAsync(string virtualCourtRoomId,
            CreateConsultationRoomParams createConsultationRoomParams)
        {
            var consultationRoomResponse = new CreateConsultationRoomResponse
            {
                Room_label = $"JudgeConsultationRoom {DateTime.UtcNow.Ticks.ToString()}"
            };
            return Task.FromResult(consultationRoomResponse);
        }
        
        public Task TransferParticipantAsync(Guid conferenceId, Guid participantId, string fromRoom, string toRoom)
        {
            return Task.CompletedTask;
        }
    }
}
