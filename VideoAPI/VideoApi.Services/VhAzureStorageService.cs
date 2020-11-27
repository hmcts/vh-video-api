using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using VideoApi.Common.Configuration;
using VideoApi.Services.Contracts;

namespace VideoApi.Services
{
    public class VhAzureStorageService : AzureStorageServiceBase, IAzureStorageService
    {
        public VhAzureStorageService(BlobServiceClient serviceClient, WowzaConfiguration wowzaConfig, bool useUserDelegation)
        : base(serviceClient, wowzaConfig, useUserDelegation)
        {
        }

        public AzureStorageServiceType AzureStorageServiceType { get; } = AzureStorageServiceType.Vh;
    }
}
