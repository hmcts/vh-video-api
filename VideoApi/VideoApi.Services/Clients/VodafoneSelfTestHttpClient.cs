using Microsoft.Extensions.Logging;
using VideoApi.Services.Contracts;

namespace VideoApi.Services.Clients;

public interface IVodafoneSelfTestHttpClient : ISupplierSelfTestHttpClient;

public class VodafoneSelfTestHttpClient(ISupplierApiClient supplierApiClient, ILogger<VodafoneSelfTestHttpClient> logger)
    : SupplierSelfTestHttpClient(supplierApiClient, logger);
       
