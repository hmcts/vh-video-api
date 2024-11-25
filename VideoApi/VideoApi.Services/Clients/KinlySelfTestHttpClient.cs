using System.Net.Http;
using Microsoft.Extensions.Logging;
using VideoApi.Common.Security.Supplier.Kinly;
using VideoApi.Services.Contracts;

namespace VideoApi.Services.Clients;

public interface IKinlySelfTestHttpClient : ISupplierSelfTestHttpClient;

public class KinlySelfTestHttpClient(HttpClient httpClient, KinlyConfiguration config, ILogger<KinlySelfTestHttpClient> logger)
    : SupplierSelfTestHttpClient(httpClient, config, logger);
       
