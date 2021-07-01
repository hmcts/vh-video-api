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

        Task<List<Azure.Storage.Blobs.Models.BlobItem>> GetAllBlobsAsync(string filePathPrefix);

        Task<List<string>> GetAllBlobsAsync2(string filePathPrefix, string date, string caseReference);

        Task<IEnumerable<string>> GetAllBlobNamesByFilePathPrefix(string filePathPrefix);

    }
}
