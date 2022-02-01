using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using System.Threading.Tasks;

namespace VideoApi.Services
{
    public class BlobClientExtension : IBlobClientExtension
    {
        public async Task<BlobProperties> GetPropertiesAsync(BlobClient blobClient)
        {
            var response = await blobClient.GetPropertiesAsync();
            return response.Value;
        }
    }
}
