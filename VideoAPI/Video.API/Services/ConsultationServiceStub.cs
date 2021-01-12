using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using VideoApi.Contract.Requests;
using VideoApi.Domain;
using VideoApi.Domain.Enums;
using VideoApi.Services.Contracts;
using Task = System.Threading.Tasks.Task;

namespace Video.API.Services
{
    [ExcludeFromCodeCoverage]
    public class ConsultationServiceStub : IConsultationService
    {
        public Task<Room> GetAvailableConsultationRoomAsync(StartConsultationRequest request)
        {
            var room = new Room(Guid.NewGuid(), "Judge", VirtualCourtRoomType.JudgeJOH);
            return Task.FromResult(room);
        }
        
        public Task<IActionResult> TransferParticipantToConsultationRoomAsync(StartConsultationRequest request, Room room)
        {
            return Task.FromResult((IActionResult)new AcceptedResult());
        }
    }
}
