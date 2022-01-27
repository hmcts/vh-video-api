using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Sas;
using VideoApi.Common.Configuration;
using VideoApi.Services.Contracts;
using VideoApi.Services.Exceptions;

namespace VideoApi.Services
{
    public class AzureStorageServiceBase :  IAzureStorageService
    {
        private readonly BlobServiceClient _serviceClient;

        private readonly IBlobStorageConfiguration _blobStorageConfiguration;

        private readonly bool _useUserDelegation;

        private readonly IBlobClientExtension _blobClientExtension;

        public AzureStorageServiceType AzureStorageServiceType => throw new NotImplementedException();

        protected AzureStorageServiceBase(BlobServiceClient serviceClient, IBlobStorageConfiguration blobStorageConfiguration, IBlobClientExtension blobClientExtension, bool useUserDelegation)
        {
            _serviceClient = serviceClient;
            _blobStorageConfiguration = blobStorageConfiguration;
            _useUserDelegation = useUserDelegation;
            _blobClientExtension = blobClientExtension;
        }

        public Task<bool> FileExistsAsync(string filePath) => ExistsAsync(filePath, _blobStorageConfiguration.StorageContainerName);

        public Task<string> CreateSharedAccessSignature(string filePath, TimeSpan validUntil) => GenerateSharedAccessSignature(filePath,
                _blobStorageConfiguration.StorageContainerName,
                _blobStorageConfiguration.StorageEndpoint,
                _blobStorageConfiguration.StorageAccountName,
                _blobStorageConfiguration.StorageAccountKey,
                validUntil,
                _useUserDelegation);

        public async IAsyncEnumerable<BlobClient> GetAllBlobsAsync(string filePathPrefix)
        {
            var container = _serviceClient.GetBlobContainerClient(_blobStorageConfiguration.StorageContainerName);
            await foreach (var page in container.GetBlobsAsync(prefix: filePathPrefix))
            {
                yield return container.GetBlobClient(page.Name);
            }
        }

        public Task<IEnumerable<string>> GetAllBlobNamesByFilePathPrefix(string filePathPrefix)
        {
            var allBlobsAsync = GetAllBlobsAsync(filePathPrefix);
            return GetAllBlobNamesByFileExtension(allBlobsAsync);
        }

        private async Task<IEnumerable<string>> GetAllBlobNamesByFileExtension(IAsyncEnumerable<BlobClient> allBlobs, string fileExtension = ".mp4")
        {
            var blobFullNames = new List<string>();
            await foreach (var blob in allBlobs)
            {
                if (blob.Name.ToLower().EndsWith(fileExtension.ToLower()))
                {
                    blobFullNames.Add(blob.Name);
                }
            }
            return blobFullNames;
        }

        public Task<IEnumerable<string>> GetAllEmptyBlobsByFilePathPrefix(string filePathPrefix)
        {
            var allBlobsAsync = GetAllBlobsAsync(filePathPrefix);
            return GetAllEmptyBlobs(allBlobsAsync);
        }

        private async Task<IEnumerable<string>> GetAllEmptyBlobs(IAsyncEnumerable<BlobClient> allBlobs, string fileExtension = ".mp4")
        {
            var blobFullNames = new List<string>();

            await foreach (var blob in allBlobs)
            {
                if (blob.Name.ToLower().EndsWith(fileExtension.ToLower()))
                {
                    var properties = await _blobClientExtension.GetPropertiesAsync(blob);
                    
                    if (properties.ContentLength <= 0) blobFullNames.Add(blob.Name);
                }
            }

            return blobFullNames;
        }

        

        private async Task<string> GenerateSharedAccessSignature(string filePath,
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
            var token = await GenerateSasToken(builder, useUserDelegation, storageAccountName, storageAccountKey);
            return $"{storageEndpoint}{storageContainerName}/{filePath}?{token}";
        }
               
        private async Task<bool> ExistsAsync(string filePath, string storageContainerName)
        {
            var containerClient = _serviceClient.GetBlobContainerClient(storageContainerName);
            var blobClient = containerClient.GetBlobClient(filePath);

            var response = await blobClient.ExistsAsync();
            return response.Value;
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

        public async Task<bool> ReconcileFilesInStorage(string fileNamePrefix, int count)
        {
            var allBlobs = await GetAllBlobNamesByFilePathPrefix(fileNamePrefix);
            var emptyBlobs = await GetAllEmptyBlobsByFilePathPrefix(fileNamePrefix);

            if (allBlobs.Count() < count || !allBlobs.Any())
            {
                var msg = $"ReconcileFilesInStorage - File name prefix :" + fileNamePrefix + "  Expected: " + count + " Actual:" + allBlobs.Count().ToString();
                throw new AudioPlatformFileNotFoundException(msg, HttpStatusCode.NotFound);
            }

            if (emptyBlobs.Any())
            {
                var msg = $"ReconcileFilesInStorage - File name prefix :" + fileNamePrefix + "  Expected: " + count + " Actual:" + allBlobs.Count().ToString();

                foreach (var item in emptyBlobs)
                {
                    msg += string.Format(" Empty audio file : {0} ", item);
                }

                throw new AudioPlatformFileNotFoundException(msg, HttpStatusCode.NotFound);
            }

            return true;

        }
    }
}
