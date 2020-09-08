using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Sas;
using VideoApi.Common.Configuration;
using VideoApi.Services.Contracts;

namespace VideoApi.Services
{
    public class CvpAzureStorageService : IAzureStorageService
    {
        private readonly BlobServiceClient _serviceClient;
        private readonly CvpConfiguration _configuration;
        private readonly bool _useUserDelegation;

        public CvpAzureStorageService(BlobServiceClient serviceClient, CvpConfiguration configuration, bool useUserDelegation)
        {
            _serviceClient = serviceClient;
            _configuration = configuration;
            _useUserDelegation = useUserDelegation;
        }

        public AzureStorageServiceType AzureStorageServiceType { get; } = AzureStorageServiceType.Cvp;

        public async Task<bool> FileExistsAsync(string filePath)
        {
            var containerClient = _serviceClient.GetBlobContainerClient(_configuration.StorageContainerName);
            var blobClient = containerClient.GetBlobClient(filePath);

            return await blobClient.ExistsAsync();
        }

        public async Task<string> CreateSharedAccessSignature(string filePath, TimeSpan validUntil)
        {
            var now = DateTimeOffset.UtcNow;
            var until = now + validUntil;
            
            var builder = new BlobSasBuilder
            {
                BlobContainerName = _configuration.StorageContainerName,
                BlobName = filePath,
                Resource = "b",
                StartsOn = now.AddHours(-1),
                ExpiresOn = until
            };
	
            builder.SetPermissions(BlobSasPermissions.Read);
	
            return $"{_configuration.StorageEndpoint}{_configuration.StorageContainerName}/{filePath}?{await GenerateSasToken(builder)}";
        }

        public async IAsyncEnumerable<BlobClient> GetAllBlobsAsync(string filePathPrefix)
        {
            var container = _serviceClient.GetBlobContainerClient(_configuration.StorageContainerName);
            await foreach (var page in container.GetBlobsAsync(prefix: filePathPrefix))
            {
                yield return container.GetBlobClient(page.Name);
            }
        }

        private async Task<string> GenerateSasToken(BlobSasBuilder builder)
        {
            var userDelegationStart = DateTimeOffset.UtcNow.AddHours(-1);
            var userDelegationEnd = userDelegationStart.AddDays(3);
            var blobSasQueryParameters = _useUserDelegation
                ? builder.ToSasQueryParameters(await _serviceClient.GetUserDelegationKeyAsync(userDelegationStart, userDelegationEnd), _configuration.StorageAccountName)
                : builder.ToSasQueryParameters(new StorageSharedKeyCredential(_configuration.StorageAccountName, _configuration.StorageAccountKey));

            return blobSasQueryParameters.ToString();
        }
    }
}
