using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using System.Threading;
using System.Threading.Tasks;

namespace VideoApi.Services
{
    public interface IBlobClientExtension
    {
        public  Task<BlobProperties> GetPropertiesAsync(BlobClient blobClient);
    }
}
