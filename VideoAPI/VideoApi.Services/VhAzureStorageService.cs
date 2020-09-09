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
        private readonly BlobServiceClient _serviceClient;
        private readonly WowzaConfiguration _wowzaConfig;
        private readonly bool _useUserDelegation;

        public VhAzureStorageService(BlobServiceClient serviceClient, WowzaConfiguration wowzaConfig, bool useUserDelegation)
        : base(serviceClient)
        {
            _serviceClient = serviceClient;
            _wowzaConfig = wowzaConfig;
            _useUserDelegation = useUserDelegation;
        }

        public AzureStorageServiceType AzureStorageServiceType { get; } = AzureStorageServiceType.Vh;

        public async Task<bool> FileExistsAsync(string filePath)
        {
            return await ExistsAsync(filePath, _wowzaConfig.StorageContainerName);
        }

        public async Task<string> CreateSharedAccessSignature(string filePath, TimeSpan validUntil)
        {
            return await GenerateSharedAccessSignature(filePath,
                _wowzaConfig.StorageContainerName,
                _wowzaConfig.StorageEndpoint,
                _wowzaConfig.StorageAccountName,
                _wowzaConfig.StorageAccountKey,
                validUntil,
                _useUserDelegation);
        }

        public async IAsyncEnumerable<BlobClient> GetAllBlobsAsync(string filePathPrefix)
        {
            var container = _serviceClient.GetBlobContainerClient(_wowzaConfig.StorageContainerName);
            await foreach (var page in container.GetBlobsAsync(prefix: filePathPrefix))
            {
                yield return container.GetBlobClient(page.Name);
            }
        }
    }
}
