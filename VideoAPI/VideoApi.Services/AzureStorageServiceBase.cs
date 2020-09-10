using System;
using System.Threading.Tasks;
using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Sas;

namespace VideoApi.Services
{
    public class AzureStorageServiceBase
    {
        private readonly BlobServiceClient _serviceClient;

        protected AzureStorageServiceBase(BlobServiceClient serviceClient)
        {
            _serviceClient = serviceClient;
        }
        
        protected async Task<string> GenerateSharedAccessSignature(string filePath, 
            string storageContainerName, 
            string storageEndpoint, 
            string storageAccountName, 
            string storageAccountKey,
            TimeSpan validUntil,
            bool useUserDelegation)
        {
            var now = DateTimeOffset.UtcNow;
            var until = now + validUntil;

            var builder = new BlobSasBuilder
            {
                BlobContainerName = storageContainerName,
                BlobName = filePath,
                Resource = "b",
                StartsOn = now.AddHours(-1),
                ExpiresOn = until
            };

            builder.SetPermissions(BlobSasPermissions.Read);

            return $"{storageEndpoint}{storageContainerName}/{filePath}?{await GenerateSasToken(builder, useUserDelegation, storageAccountName, storageAccountKey)}";
        }

        protected async Task<bool> ExistsAsync(string filePath, string storageContainerName)
        {
            var containerClient = _serviceClient.GetBlobContainerClient(storageContainerName);
            var blobClient = containerClient.GetBlobClient(filePath);

            return await blobClient.ExistsAsync();
        }

        private async Task<string> GenerateSasToken(BlobSasBuilder builder, bool useUserDelegation, string storageAccountName, string storageAccountKey)
        {
            var userDelegationStart = DateTimeOffset.UtcNow.AddHours(-1);
            var userDelegationEnd = userDelegationStart.AddDays(3);
            var blobSasQueryParameters = useUserDelegation
                ? builder.ToSasQueryParameters(await _serviceClient.GetUserDelegationKeyAsync(userDelegationStart, userDelegationEnd), storageAccountName)
                : builder.ToSasQueryParameters(new StorageSharedKeyCredential(storageAccountName, storageAccountKey));

            return blobSasQueryParameters.ToString();
        }
    }
}
