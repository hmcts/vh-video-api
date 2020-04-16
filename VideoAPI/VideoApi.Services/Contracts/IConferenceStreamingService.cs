using System;
using System.Threading.Tasks;

namespace VideoApi.Services.Contracts
{
    public interface IConferenceStreamingService
    {
        Task CreateConferenceStreamAsync(string caseNumber, Guid conferenceId);
    }
}
