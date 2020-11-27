using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using VideoApi.Common.Configuration;
using VideoApi.Services.Contracts;

namespace VideoApi.Services
{
    public class CvpAzureStorageService : AzureStorageServiceBase, IAzureStorageService
    {
        private readonly BlobServiceClient _serviceClient;
        private readonly CvpConfiguration _cvpConfig;
        private readonly bool _useUserDelegation;

        public CvpAzureStorageService(BlobServiceClient serviceClient, CvpConfiguration cvpConfig, bool useUserDelegation)
        : base(serviceClient)
        {
            _serviceClient = serviceClient;
            _cvpConfig = cvpConfig;
            _useUserDelegation = useUserDelegation;
        }

        public AzureStorageServiceType AzureStorageServiceType { get; } = AzureStorageServiceType.Cvp;

        public async Task<bool> FileExistsAsync(string filePath)
        {
            return await ExistsAsync(filePath, _cvpConfig.StorageContainerName);
        }

        public async Task<string> CreateSharedAccessSignature(string filePath, TimeSpan validUntil)
        {
            return await GenerateSharedAccessSignature(filePath,
                _cvpConfig.StorageContainerName,
                _cvpConfig.StorageEndpoint,
                _cvpConfig.StorageAccountName,
                _cvpConfig.StorageAccountKey,
                validUntil,
                _useUserDelegation);
        }

        public async IAsyncEnumerable<BlobClient> GetAllBlobsAsync(string filePathPrefix)
        {
            var container = _serviceClient.GetBlobContainerClient(_cvpConfig.StorageContainerName);
            await foreach (var page in container.GetBlobsAsync(prefix: filePathPrefix))
            {
                yield return container.GetBlobClient(page.Name);
            }
        }

        public async Task<IEnumerable<string>> GetAllBlobNamesByFilePathPrefix(string filePathPrefix, string fileExtension = ".mp4")
        {
            var allBlobsAsync = GetAllBlobsAsync(filePathPrefix);
            var result = await GetAllBlobNamesByFileExtension(allBlobsAsync);

            return result;
        }
    }
}
