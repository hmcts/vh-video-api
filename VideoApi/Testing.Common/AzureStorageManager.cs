using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Azure;
using Azure.Storage;
using Azure.Storage.Blobs;
using NUnit.Framework;

namespace Testing.Common
{
    public class AzureStorageManager
    {
        private string _storageContainerName;
        private BlobContainerClient _blobContainerClient;
        private BlobClient _blobClient;
        
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

        public static BlobServiceClient CreateAzuriteBlobServiceClient()
        {
            var connectionString =
                "DefaultEndpointsProtocol=http;AccountName=devstoreaccount1;AccountKey=Eby8vdM02xNOcqFlqUwJPLlmEtlCDXJ1OUzFT50uSRZ6IFsuFq2UVErCz4I6tq/K1SZFPTOtr/KBHBeksoGMGw==;BlobEndpoint=http://127.0.0.1:10000/devstoreaccount1;QueueEndpoint=http://127.0.0.1:10001/devstoreaccount1;TableEndpoint=http://127.0.0.1:10002/devstoreaccount1;";
            var serviceClient = new BlobServiceClient(connectionString);
            return serviceClient;
        }

        private BlobContainerClient CreateContainerClient()
        {
            var serviceClient = CreateAzuriteBlobServiceClient();
            
            var containerClient = serviceClient.GetBlobContainerClient(_storageContainerName);
            containerClient.CreateIfNotExists();
            return containerClient;
        }
    }
}
