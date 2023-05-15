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
using VideoApi.Services.Exceptions;

namespace VideoApi.UnitTests.Services
{
    public class VhAzureStorageServiceTest
    {
        private Mock<BlobContainerClient> blobContainerClient;
        private Mock<BlobClient>  blobClientMock ;
        private Mock<BlobServiceClient> blobServiceClient ;
        private Mock<IBlobClientExtension> blobClientExtensionMock;
        private VhAzureStorageService service;
        private WowzaConfiguration config;
        private string filePathPrefix;
        private Mock<BlobContainerClient> blobContainerClientMock ;
        private BlobProperties emptyBlobClientProperties;
        private BlobProperties notEmptyBlobClientProperties;
        private Mock<AsyncPageable<BlobItem>> pageable;

        [SetUp]
        public void Setup()
        {
            blobContainerClient = new Mock<BlobContainerClient>();
            blobClientMock = new Mock<BlobClient>();
            blobServiceClient = new Mock<BlobServiceClient>();
            blobClientExtensionMock = new Mock<IBlobClientExtension>();
            blobContainerClientMock = new Mock<BlobContainerClient>();
            pageable = new Mock<AsyncPageable<BlobItem>>();
            filePathPrefix = "myFilePath";

            config = new WowzaConfiguration
            {
                StorageEndpoint = "https://container.blob.core.windows.net/", StorageContainerName = "container",
                StorageAccountName = "accountName", StorageAccountKey = "YWNjb3VudEtleQ=="
            };

            service = new VhAzureStorageService(blobServiceClient.Object,config, true, blobClientExtensionMock.Object);

            emptyBlobClientProperties = BlobsModelFactory.BlobProperties(DateTimeOffset.UtcNow,
                leaseState: LeaseState.Available,
                leaseStatus: LeaseStatus.Unlocked,
                contentLength: 0,
                LeaseDurationType.Infinite, ETag.All, new byte[] { 1 }, "", "", "", "", "",
                false, 121, CopyStatus.Success, "", new Uri("https://www.portal.azure.com"), 0, "", false, "", "", "", "",
                DateTimeOffset.UtcNow, "", BlobType.Block, false, new Dictionary<string, 
                string>(), "", DateTimeOffset.UtcNow, DateTime.UtcNow, "");

             notEmptyBlobClientProperties = BlobsModelFactory.BlobProperties(DateTimeOffset.UtcNow,
                leaseState: LeaseState.Available,
                leaseStatus: LeaseStatus.Unlocked,
                contentLength: 64355,
                LeaseDurationType.Infinite, ETag.All, new byte[] { 1 }, "", "", "", "", "",
                false, 121, CopyStatus.Success, "", new Uri("https://www.portal.azure.com"), 0, "", false, "", "", "", "",
                DateTimeOffset.UtcNow, "", BlobType.Block, false, new Dictionary<string, string>(), "", DateTimeOffset.UtcNow, DateTime.UtcNow, "");

        }

        [Test]
        public async Task FileExistsAsync_returns_true()
        {
            
            blobServiceClient.Setup(x => x.GetBlobContainerClient(It.IsAny<string>())).Returns(blobContainerClient.Object);
            blobContainerClient.Setup(x => x.GetBlobClient(It.IsAny<string>())).Returns(blobClientMock.Object);
            blobClientMock.Setup(x => x.ExistsAsync(CancellationToken.None)).ReturnsAsync(Response.FromValue<bool>(true, null));
                        
            var result = await service.FileExistsAsync(It.IsAny<string>());

            result.Should().BeTrue();
        }
        
        [Test]
        public async Task CreateSharedAccessSignature_returns_token_created_using_StorageSharedKeyCredential()
        {
            var s_service = new VhAzureStorageService(blobServiceClient.Object, config, false, blobClientExtensionMock.Object);

            var result = await s_service.CreateSharedAccessSignature("myFilePath", It.IsAny<TimeSpan>());

            result.Should().NotBeNullOrEmpty();
            result.Should().StartWith($"{config.StorageEndpoint}{config.StorageContainerName}/myFilePath?");

            blobServiceClient
                .Verify(x => x.GetUserDelegationKeyAsync(It.IsAny<DateTimeOffset>(), It.IsAny<DateTimeOffset>(), It.IsAny<CancellationToken>()), Times.Never);
        }
        
        [Test]
        public async Task CreateSharedAccessSignature_returns_token_created_using_DelegationKey()
        {
            
            blobServiceClient
                .Setup(x => x.GetUserDelegationKeyAsync(It.IsAny<DateTimeOffset>(), It.IsAny<DateTimeOffset>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Response.FromValue(BlobsModelFactory.UserDelegationKey("","","","","", DateTimeOffset.Now, DateTimeOffset.Now), null));
            
            var result = await service.CreateSharedAccessSignature("myFilePath", TimeSpan.FromDays(7));

            result.Should().NotBeNullOrEmpty();
            result.Should().StartWith($"{config.StorageEndpoint}{config.StorageContainerName}/myFilePath?");

            blobServiceClient
                .Verify(x => x.GetUserDelegationKeyAsync(It.IsAny<DateTimeOffset>(), It.IsAny<DateTimeOffset>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task GetAllBlobsAsync_returns_blob_clients()
        {
            blobServiceClient.Setup(x => x.GetBlobContainerClient(config.StorageContainerName)).Returns(blobContainerClientMock.Object);
            
            blobContainerClientMock.Setup(x => x.GetBlobsAsync(BlobTraits.None, BlobStates.None, filePathPrefix, default))
                .Returns(pageable.Object);
            pageable.Setup(x => x.GetAsyncEnumerator(default)).Returns(GetMockBlobItems());
            blobContainerClientMock.Setup(x => x.GetBlobClient(It.IsAny<string>())).Returns(blobClientMock.Object);
            
            await foreach (var item in service.GetAllBlobsAsync(filePathPrefix))
            {
                item.Should().NotBeNull();
            }
        }

        [Test]
        public async Task GetAllBlobNamesByFilePathPrefix_returns_blob_file_names()
        {
            blobClientMock.Setup(x => x.Name).Returns("SomeBlob.mp4");

            blobServiceClient.Setup(x => x.GetBlobContainerClient(config.StorageContainerName)).Returns(blobContainerClientMock.Object);
            
            blobContainerClientMock.Setup(x => x.GetBlobsAsync(BlobTraits.None, BlobStates.None, filePathPrefix, default))
                .Returns(pageable.Object);
            pageable.Setup(x => x.GetAsyncEnumerator(default)).Returns(GetMockBlobItems());
            blobContainerClientMock.Setup(x => x.GetBlobClient(It.IsAny<string>())).Returns(blobClientMock.Object);
            
            var list = await service.GetAllBlobNamesByFilePathPrefix(filePathPrefix);
            list.Should().NotBeNull();
            list.Count().Should().Be(1);
        }
        
        [Test]
        public async Task GetEmptyBlobNamesByFilePathPrefix_returns_blob_file_names_if_blob_content_length_is_zero()
        {
            pageable.Setup(x => x.GetAsyncEnumerator(default)).Returns(GetMockBlobItems());

            blobContainerClientMock.Setup(x => x.GetBlobsAsync(BlobTraits.None, BlobStates.None, filePathPrefix, default)).Returns(pageable.Object);
            blobContainerClientMock.Setup(x => x.GetBlobClient(It.IsAny<string>())).Returns(blobClientMock.Object);

            blobServiceClient.Setup(x => x.GetBlobContainerClient(config.StorageContainerName)).Returns(blobContainerClientMock.Object);
            
            blobClientExtensionMock.Setup(x => x.GetPropertiesAsync(It.IsAny<BlobClient>())).ReturnsAsync(emptyBlobClientProperties);
            blobClientMock.Setup(x => x.Name).Returns("SomeBlob.mp4");
            
            var list = await service.GetAllEmptyBlobsByFilePathPrefix(filePathPrefix);

            list.Should().NotBeNull();
            list.Count().Should().Be(1);
        }

        [Test]
        public async Task GetEmptyBlobNamesByFilePathPrefix_returns_empty_blob_file_names_if_blob_content_length_is__not_zero()
        {
            pageable.Setup(x => x.GetAsyncEnumerator(default)).Returns(GetMockBlobItems());

            blobContainerClientMock.Setup(x => x.GetBlobsAsync(BlobTraits.None, BlobStates.None, filePathPrefix, default)).Returns(pageable.Object);
            blobContainerClientMock.Setup(x => x.GetBlobClient(It.IsAny<string>())).Returns(blobClientMock.Object);

            blobServiceClient.Setup(x => x.GetBlobContainerClient(config.StorageContainerName)).Returns(blobContainerClientMock.Object);

            blobClientExtensionMock.Setup(x => x.GetPropertiesAsync(It.IsAny<BlobClient>())).ReturnsAsync(notEmptyBlobClientProperties);
            blobClientMock.Setup(x => x.Name).Returns("SomeBlob.mp4");

            var result = await service.GetAllEmptyBlobsByFilePathPrefix(filePathPrefix);

            result.Should().BeEmpty();
            result.Count().Should().Be(0);
        }


        [Test]
        public async Task Should_Check_Storage_For_Files_With_Prefix_And_Match_Count()
        {           
            pageable.Setup(x => x.GetAsyncEnumerator(default)).Returns(GetMockBlobItems());

            blobContainerClientMock.Setup(x => x.GetBlobsAsync(BlobTraits.None, BlobStates.None, filePathPrefix, default)).Returns(pageable.Object);
            blobContainerClientMock.Setup(x => x.GetBlobClient(It.IsAny<string>())).Returns(blobClientMock.Object);

            blobClientExtensionMock.Setup(x => x.GetPropertiesAsync(It.IsAny<BlobClient>())).ReturnsAsync(notEmptyBlobClientProperties);

            blobServiceClient.Setup(x => x.GetBlobContainerClient(config.StorageContainerName)).Returns(blobContainerClientMock.Object);

            blobClientMock.Setup(x => x.Name).Returns("SomeBlob.mp4");
                        
            var result = await service.ReconcileFilesInStorage("myFilePath", 1);

            result.Should().Be(true);
        }

        [Test]
        public void Should_Throw_AudioPlatformFileNotFoundException_If_Storage_Returns_Less_Files_With_Parameter_FileCount()
        {
            pageable.Setup(x => x.GetAsyncEnumerator(default)).Returns(GetEmptyMockBlobItems());

            blobContainerClientMock.Setup(x => x.GetBlobsAsync(BlobTraits.None, BlobStates.None, filePathPrefix, default)).Returns(pageable.Object);

            blobContainerClientMock.Setup(x => x.GetBlobClient(It.IsAny<string>())).Returns(blobClientMock.Object);

            blobClientExtensionMock.Setup(x => x.GetPropertiesAsync(It.IsAny<BlobClient>())).ReturnsAsync(notEmptyBlobClientProperties);

            blobServiceClient.Setup(x => x.GetBlobContainerClient(config.StorageContainerName)).Returns(blobContainerClientMock.Object);

            blobClientMock.Setup(x => x.Name).Returns(filePathPrefix+ ".mp4");

            var msg = $"ReconcileFilesInStorage - File name prefix :" + filePathPrefix + "  Expected: " + "1" + " Actual:" + "0";

            Assert.That(async () => await service.ReconcileFilesInStorage("myFilePath", 1), Throws.TypeOf<AudioPlatformFileNotFoundException>().With.Message.EqualTo(msg));

        }

        private async IAsyncEnumerator<BlobItem> GetMockBlobItems()
        {
            var blobItem = new Mock<BlobItem>();

            yield return blobItem.Object;

            await Task.CompletedTask;
        }
       
        private async IAsyncEnumerator<BlobItem> GetEmptyMockBlobItems()
        {
            await Task.CompletedTask;
            yield break;

        }
    }
}
