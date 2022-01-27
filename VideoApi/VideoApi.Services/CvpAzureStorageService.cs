using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using VideoApi.Common.Configuration;
using VideoApi.Services.Contracts;

namespace VideoApi.Services
{
    public class CvpAzureStorageService : AzureStorageServiceBase
    {
        public CvpAzureStorageService(BlobServiceClient serviceClient, CvpConfiguration cvpConfig, bool useUserDelegation, IBlobClientExtension blobClientExtension)
        : base(serviceClient, cvpConfig, blobClientExtension, useUserDelegation)
        {
        }

        public AzureStorageServiceType AzureStorageServiceType { get; } = AzureStorageServiceType.Cvp;
    }
}
