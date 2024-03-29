using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Azure.Storage.Blobs;

namespace VideoApi.Services.Contracts
{
    public interface IAzureStorageService
    {
        AzureStorageServiceType AzureStorageServiceType { get; }

        Task<bool> FileExistsAsync(string filePath);

        Task<string> CreateSharedAccessSignature(string filePath, TimeSpan validUntil);

        IAsyncEnumerable<BlobClient> GetAllBlobsAsync(string filePathPrefix);

        Task<IEnumerable<string>> GetAllBlobNamesByFilePathPrefix(string filePathPrefix);

        Task<IEnumerable<string>> GetAllEmptyBlobsByFilePathPrefix(string filePathPrefix);

        Task<bool> ReconcileFilesInStorage(string fileNamePrefix, int count);
    }
}
