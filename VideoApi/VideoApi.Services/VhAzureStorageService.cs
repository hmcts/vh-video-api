using Azure.Storage.Blobs;
using VideoApi.Common.Configuration;
using VideoApi.Services.Contracts;

namespace VideoApi.Services
{
    public class VhAzureStorageService : AzureStorageServiceBase
    {
        public VhAzureStorageService(BlobServiceClient serviceClient, WowzaConfiguration wowzaConfig, bool useUserDelegation, IBlobClientExtension blobClientExtension)
        : base(serviceClient, wowzaConfig, blobClientExtension, useUserDelegation )
        {
        }

        public AzureStorageServiceType AzureStorageServiceType { get; } = AzureStorageServiceType.Vh;
    }
}
