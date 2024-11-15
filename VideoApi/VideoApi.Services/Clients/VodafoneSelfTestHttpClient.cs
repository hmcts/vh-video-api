using System.Net.Http;
using Microsoft.Extensions.Logging;
using VideoApi.Common.Security.Supplier.Vodafone;
using VideoApi.Services.Contracts;

namespace VideoApi.Services.Clients;

public interface IVodafoneSelfTestHttpClient : ISupplierSelfTestHttpClient;

public class VodafoneSelfTestHttpClient(HttpClient httpClient, VodafoneConfiguration config, ILogger<VodafoneSelfTestHttpClient> logger)
    : SupplierSelfTestHttpClient(httpClient, config, logger);
       
