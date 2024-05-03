using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Azure;
using Azure.Storage;
using Azure.Storage.Blobs;
using NUnit.Framework;

namespace Testing.Common.AcCommon
{
    public class AzureStorageManager
    {
        private string _storageAccountName;
        private string _storageAccountKey;
        private string _storageContainerName;
        private BlobContainerClient _blobContainerClient;
        private BlobClient _blobClient;

        public AzureStorageManager SetStorageAccountName(string storageAccountName)
        {
            _storageAccountName = storageAccountName;
            return this;
        }

        public AzureStorageManager SetStorageAccountKey(string storageAccountKey)
        {
            _storageAccountKey = storageAccountKey;
            return this;
        }

        public AzureStorageManager SetStorageContainerName(string storageContainerName)
        {
            _storageContainerName = storageContainerName;
            return this;
        }

        public AzureStorageManager CreateBlobClient(string filePathWithoutExtension)
        {
            _blobContainerClient = CreateContainerClient();
            _blobClient = _blobContainerClient.GetBlobClient($"{filePathWithoutExtension}.mp4");
            return this;
        }

        public AzureStorageManager CreateBlobContainerClient()
        {
            _blobContainerClient = CreateContainerClient();
            return this;
        }

        public async IAsyncEnumerable<BlobClient> GetAllBlobsAsync(string filePathNamePrefix)
        {
            await foreach (var page in _blobContainerClient.GetBlobsAsync(prefix: filePathNamePrefix))
            {
                yield return _blobContainerClient.GetBlobClient(page.Name);
            }
        }

        public async Task UploadAudioFileToStorage(string file)
        {
            await _blobClient.UploadAsync(file);

            if (!await _blobClient.ExistsAsync())
            {
                throw new RequestFailedException($"Can not find file: {file}");
            }

            TestContext.WriteLine($"Uploaded audio file to : {file}");
        }

        public async Task UploadFileToStorage(string localFileName, string filePathOnStorage)
        {
            var blobClient = _blobContainerClient.GetBlobClient(filePathOnStorage);
            
            await blobClient.UploadAsync(localFileName);

            if (!await blobClient.ExistsAsync())
            {
                throw new RequestFailedException($"Can not find file: {localFileName} with full path {filePathOnStorage}");
            }

            TestContext.WriteLine($"Uploaded audio file to : {localFileName} with full path: {filePathOnStorage}");
        }

        public async Task<bool> VerifyAudioFileExistsInStorage()
        {
            return await _blobClient.ExistsAsync();
        }

        public async Task RemoveAudioFileFromStorage()
        {
            await _blobClient.DeleteAsync();
            TestContext.WriteLine("Deleted audio file");
        }

        private BlobContainerClient CreateContainerClient()
        {
            var storageSharedKeyCredential = new StorageSharedKeyCredential(_storageAccountName, _storageAccountKey);
            var serviceEndpoint = $"https://{_storageAccountName}.blob.core.windows.net/";
            var serviceClient = new BlobServiceClient(new Uri(serviceEndpoint), storageSharedKeyCredential);
            return serviceClient.GetBlobContainerClient(_storageContainerName);
        }
    }
}
