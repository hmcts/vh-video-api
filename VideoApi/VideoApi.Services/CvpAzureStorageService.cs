using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using VideoApi.Common.Configuration;
using VideoApi.Services.Contracts;

namespace VideoApi.Services
{
    public class CvpAzureStorageService : AzureStorageServiceBase, IAzureStorageService
    {
        public CvpAzureStorageService(BlobServiceClient serviceClient, CvpConfiguration cvpConfig, bool useUserDelegation)
        : base(serviceClient, cvpConfig, useUserDelegation)
        {
        }

        public AzureStorageServiceType AzureStorageServiceType { get; } = AzureStorageServiceType.Cvp;
    }
}
