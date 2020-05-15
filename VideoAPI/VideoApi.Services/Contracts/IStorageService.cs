using System;
using System.Threading.Tasks;

namespace VideoApi.Services.Contracts
{
    public interface IStorageService
    {
        Task<bool> FileExistsAsync(string filePath);
        Task<string> CreateSharedAccessSignature(string filePath, TimeSpan validUntil);
    }
}
