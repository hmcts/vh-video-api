using System;
using System.Collections.Generic;
using System.Linq;
using VideoApi.Services.Contracts;

namespace VideoApi.Services.Factories
{
    public class AzureStorageServiceFactory : IAzureStorageServiceFactory
    {
        private readonly IEnumerable<IAzureStorageService> _azureStorageServices;

        public AzureStorageServiceFactory(IEnumerable<IAzureStorageService> azureStorageServices)
        {
            _azureStorageServices = azureStorageServices;
        }
        
        public IAzureStorageService Create(AzureStorageServiceType azureStorageServiceType)
        {
            var azureStorageService = _azureStorageServices.FirstOrDefault(x => x.AzureStorageServiceType == azureStorageServiceType);

            if (azureStorageService == null)
            {
                throw new NotImplementedException($"Can not find azureStorageService: {azureStorageServiceType}");
            }

            return azureStorageService;
        }
    }
}
