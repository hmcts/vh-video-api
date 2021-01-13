using System;
using System.Diagnostics.CodeAnalysis;
using VideoApi.Contract.Requests;
using VideoApi.Services.Contracts;
using Task = System.Threading.Tasks.Task;

namespace VideoApi.Services
{
    [ExcludeFromCodeCoverage]
    public class ConsultationServiceStub : IConsultationService
    {


        public Task LeaveConsultationAsync(LeaveConsultationRequest request, string fromRoom, string toRoom)
        {
            return Task.CompletedTask;
        }
    }
}
