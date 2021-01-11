using System;
using System.Diagnostics.CodeAnalysis;
using VideoApi.Domain;
using VideoApi.Services.Contracts;
using Task = System.Threading.Tasks.Task;

namespace VideoApi.Services
{
    [ExcludeFromCodeCoverage]
    public class ConsultationServiceStub : IConsultationService
    {

        public Task EndJudgeJohConsultationAsync(Guid conferenceId, Room room)
        {
            return Task.CompletedTask;
        }
    }
}
