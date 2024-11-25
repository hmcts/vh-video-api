using System;
using System.Threading.Tasks;
using VideoApi.Domain;

namespace VideoApi.Services.Contracts;

public interface ISupplierSelfTestHttpClient
{
    Task<TestCallResult> GetTestCallScoreAsync(Guid participantId);
}
