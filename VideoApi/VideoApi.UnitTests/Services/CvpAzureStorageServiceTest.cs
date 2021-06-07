using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using VideoApi.Common.Configuration;
using VideoApi.Services;

namespace VideoApi.UnitTests.Services
{
    public class CvpAzureStorageServiceTest
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
            var service = new CvpAzureStorageService(blobServiceClient.Object, new CvpConfiguration(), false);

            var result = await service.FileExistsAsync(It.IsAny<string>());

            result.Should().BeTrue();
        }
        
        [Test]
        public async Task CreateSharedAccessSignature_returns_token_created_using_StorageSharedKeyCredential()
        {
            var config = new CvpConfiguration()
            {
                StorageEndpoint = "https://container.blob.core.windows.net/", StorageContainerName = "container",
                StorageAccountName = "accountName", StorageAccountKey = "YWNjb3VudEtleQ=="
            };
            var blobServiceClient = new Mock<BlobServiceClient>();
            var service = new CvpAzureStorageService(blobServiceClient.Object, config, false);

            var result = await service.CreateSharedAccessSignature("myFilePath", It.IsAny<TimeSpan>());

            result.Should().NotBeNullOrEmpty();
            result.Should().StartWith($"{config.StorageEndpoint}{config.StorageContainerName}/myFilePath?");

            blobServiceClient
                .Verify(x => x.GetUserDelegationKeyAsync(It.IsAny<DateTimeOffset>(), It.IsAny<DateTimeOffset>(), It.IsAny<CancellationToken>()), Times.Never);
        }
        
        [Test]
        public async Task CreateSharedAccessSignature_returns_token_created_using_DelegationKey()
        {
            var config = new CvpConfiguration()
            {
                StorageEndpoint = "https://container.blob.core.windows.net/", StorageContainerName = "container",
                StorageAccountName = "accountName", StorageAccountKey = "YWNjb3VudEtleQ=="
            };
            
            var blobServiceClient = new Mock<BlobServiceClient>();
            blobServiceClient
                .Setup(x => x.GetUserDelegationKeyAsync(It.IsAny<DateTimeOffset>(), It.IsAny<DateTimeOffset>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Response.FromValue(BlobsModelFactory.UserDelegationKey("","","","","", DateTimeOffset.Now, DateTimeOffset.Now), null));
            
            var service = new CvpAzureStorageService(blobServiceClient.Object, config, true);

            var result = await service.CreateSharedAccessSignature("myFilePath", TimeSpan.FromDays(7));

            result.Should().NotBeNullOrEmpty();
            result.Should().StartWith($"{config.StorageEndpoint}{config.StorageContainerName}/myFilePath?");

            blobServiceClient
                .Verify(x => x.GetUserDelegationKeyAsync(It.IsAny<DateTimeOffset>(), It.IsAny<DateTimeOffset>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task GetAllBlobsAsync_returns_blob_clients()
        {
            var config = new CvpConfiguration()
            {
                StorageEndpoint = "https://container.blob.core.windows.net/", StorageContainerName = "container",
                StorageAccountName = "accountName", StorageAccountKey = "YWNjb3VudEtleQ=="
            };

            var filePathPrefix = "myFilePath";
            var blobServiceClient = new Mock<BlobServiceClient>();
            var blobContainerClientMock = new Mock<BlobContainerClient>();
            var blobClientMock = new Mock<BlobClient>();
            blobServiceClient.Setup(x => x.GetBlobContainerClient(config.StorageContainerName)).Returns(blobContainerClientMock.Object);
            var pageable = new Mock<AsyncPageable<BlobItem>>();
            
            blobContainerClientMock.Setup(x => x.GetBlobsAsync(BlobTraits.None, BlobStates.None, filePathPrefix, default))
                .Returns(pageable.Object);
            pageable.Setup(x => x.GetAsyncEnumerator(default)).Returns(GetMockBlobItems());
            blobContainerClientMock.Setup(x => x.GetBlobClient(It.IsAny<string>())).Returns(blobClientMock.Object);
            
            var service = new CvpAzureStorageService(blobServiceClient.Object, config, true);

            foreach (var item in await service.GetAllBlobsAsync(filePathPrefix))
            {
                item.Should().NotBeNull();
            }
        }

        [Test]
        public async Task GetAllBlobNamesByFilePathPrefix_returns_blob_files_names()
        {
            var config = new CvpConfiguration()
            {
                StorageEndpoint = "https://container.blob.core.windows.net/", StorageContainerName = "container",
                StorageAccountName = "accountName", StorageAccountKey = "YWNjb3VudEtleQ=="
            };

            var filePathPrefix = "myFilePath";
            var blobServiceClient = new Mock<BlobServiceClient>();
            var blobContainerClientMock = new Mock<BlobContainerClient>();
            var blobClientMock = new Mock<BlobClient>();
            blobClientMock.Setup(x => x.Name).Returns("SomeBlob.mp4");
            blobServiceClient.Setup(x => x.GetBlobContainerClient(config.StorageContainerName)).Returns(blobContainerClientMock.Object);
            var pageable = new Mock<AsyncPageable<BlobItem>>();

            blobContainerClientMock.Setup(x => x.GetBlobsAsync(BlobTraits.None, BlobStates.None, filePathPrefix, default))
                .Returns(pageable.Object);
            pageable.Setup(x => x.GetAsyncEnumerator(default)).Returns(GetMockBlobItems());
            blobContainerClientMock.Setup(x => x.GetBlobClient(It.IsAny<string>())).Returns(blobClientMock.Object);

            var service = new CvpAzureStorageService(blobServiceClient.Object, config, true);

            var list = await service.GetAllBlobNamesByFilePathPrefix(filePathPrefix);
            list.Should().NotBeNull();
            list.Count().Should().Be(1);
        }

        private async IAsyncEnumerator<BlobItem> GetMockBlobItems()
        {
            var blobItem = new Mock<BlobItem>();
            
            yield return blobItem.Object;

            await Task.CompletedTask;
        }
    }
}
