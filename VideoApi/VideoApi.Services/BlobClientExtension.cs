using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using System.Threading.Tasks;

namespace VideoApi.Services
{
    public class BlobClientExtension : IBlobClientExtension
    {
        public async Task<Response<BlobProperties>> GetPropertiesAsync(BlobClient blobClient)
        {
            return await blobClient.GetPropertiesAsync();
        }
    }
}
