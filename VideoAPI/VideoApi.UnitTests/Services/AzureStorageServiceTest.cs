using System;
using System.Threading;
using System.Threading.Tasks;
using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using VideoApi.Common.Configuration;
using VideoApi.Services;

namespace VideoApi.UnitTests.Services
{
    public class AzureStorageServiceTest
    {
        [Test]
        public async Task FileExistsAsync_returns_true()
        {
            var blobContainerClient = new Mock<BlobContainerClient>();
            var blobClient = new Mock<BlobClient>();
            var blobServiceClient = new Mock<BlobServiceClient>();
            blobServiceClient.Setup(x => x.GetBlobContainerClient(It.IsAny<string>())).Returns(blobContainerClient.Object);
            blobContainerClient.Setup(x => x.GetBlobClient(It.IsAny<string>())).Returns(blobClient.Object);
            blobClient.Setup(x => x.ExistsAsync(CancellationToken.None)).ReturnsAsync(Response.FromValue<bool>(true, null));
            var service = new VhAzureStorageService(blobServiceClient.Object, new WowzaConfiguration(), false);

            var result = await service.FileExistsAsync(It.IsAny<string>());

            result.Should().BeTrue();
        }
        
        [Test]
        public async Task CreateSharedAccessSignature_returns_token_created_using_StorageSharedKeyCredential()
        {
            var config = new WowzaConfiguration
            {
                StorageEndpoint = "https://container.blob.core.windows.net/", StorageContainerName = "container",
                StorageAccountName = "accountName", StorageAccountKey = "YWNjb3VudEtleQ=="
            };
            var blobServiceClient = new Mock<BlobServiceClient>();
            var service = new VhAzureStorageService(blobServiceClient.Object, config, false);

            var result = await service.CreateSharedAccessSignature("myFilePath", It.IsAny<TimeSpan>());

            result.Should().NotBeNullOrEmpty();
            result.Should().StartWith($"{config.StorageEndpoint}{config.StorageContainerName}/myFilePath?");

            blobServiceClient
                .Verify(x => x.GetUserDelegationKeyAsync(It.IsAny<DateTimeOffset>(), It.IsAny<DateTimeOffset>(), It.IsAny<CancellationToken>()), Times.Never);
        }
        
        [Test]
        public async Task CreateSharedAccessSignature_returns_token_created_using_DelegationKey()
        {
            var config = new WowzaConfiguration
            {
                StorageEndpoint = "https://container.blob.core.windows.net/", StorageContainerName = "container",
                StorageAccountName = "accountName", StorageAccountKey = "YWNjb3VudEtleQ=="
            };
            
            var blobServiceClient = new Mock<BlobServiceClient>();
            blobServiceClient
                .Setup(x => x.GetUserDelegationKeyAsync(It.IsAny<DateTimeOffset>(), It.IsAny<DateTimeOffset>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Response.FromValue(BlobsModelFactory.UserDelegationKey("","","","","", DateTimeOffset.Now, DateTimeOffset.Now), null));
            
            var service = new VhAzureStorageService(blobServiceClient.Object, config, true);

            var result = await service.CreateSharedAccessSignature("myFilePath", TimeSpan.FromDays(7));

            result.Should().NotBeNullOrEmpty();
            result.Should().StartWith($"{config.StorageEndpoint}{config.StorageContainerName}/myFilePath?");

            blobServiceClient
                .Verify(x => x.GetUserDelegationKeyAsync(It.IsAny<DateTimeOffset>(), It.IsAny<DateTimeOffset>(), It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
