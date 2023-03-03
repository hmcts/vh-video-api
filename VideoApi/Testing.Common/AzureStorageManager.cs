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
        private static string DefaultAzuriteConnectionString =
            "DefaultEndpointsProtocol=http;AccountName=devstoreaccount1;AccountKey=Eby8vdM02xNOcqFlqUwJPLlmEtlCDXJ1OUzFT50uSRZ6IFsuFq2UVErCz4I6tq/K1SZFPTOtr/KBHBeksoGMGw==;BlobEndpoint=http://127.0.0.1:10000/devstoreaccount1;QueueEndpoint=http://127.0.0.1:10001/devstoreaccount1;TableEndpoint=http://127.0.0.1:10002/devstoreaccount1;";
        private string _storageContainerName;
        
        private string _storageAccountName;
        private string _storageAccountKey;
        
        private BlobContainerClient _blobContainerClient;
        private BlobClient _blobClient;
        
        public AzureStorageManager()
        {
        }

        public AzureStorageManager(string accountName, string accountKey, string containerName)
        {
            _storageAccountName = accountName;
            _storageAccountKey = accountKey;
            _storageContainerName = containerName;
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
        
        public AzureStorageManager CreateBlobClient(string filePathWithoutExtension, string connectionString)
        {
            _blobContainerClient = CreateContainerClient(connectionString);
            _blobClient = _blobContainerClient.GetBlobClient($"{filePathWithoutExtension}.mp4");
            return this;
        }

        public AzureStorageManager CreateBlobContainerClient(string connectionString)
        {
            _blobContainerClient = CreateContainerClient(connectionString);
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

        public static BlobServiceClient CreateAzuriteBlobServiceClient(string connectionString)
        {
            connectionString ??= DefaultAzuriteConnectionString; 
            TestContext.WriteLine($"Azure Connection string {connectionString}");
            var serviceClient = new BlobServiceClient(connectionString, new BlobClientOptions {Retry = { MaxRetries = 2}});
            return serviceClient;
        }

        private BlobContainerClient CreateContainerClient(string connectionString)
        {
            var serviceClient = CreateAzuriteBlobServiceClient(connectionString);
            var containerClient = serviceClient.GetBlobContainerClient(_storageContainerName);
            containerClient.CreateIfNotExists();
            return containerClient;
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
