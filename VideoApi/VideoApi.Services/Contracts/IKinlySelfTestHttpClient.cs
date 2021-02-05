using System;
using System.Threading.Tasks;
using VideoApi.Domain;

namespace VideoApi.Services.Contracts
{
    public interface IKinlySelfTestHttpClient
    {
        Task<TestCallResult> GetTestCallScoreAsync(Guid participantId);
    }
}
