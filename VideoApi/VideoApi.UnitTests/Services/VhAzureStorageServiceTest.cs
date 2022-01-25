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
    public class VhAzureStorageServiceTest
    {
        [Test]
        public async Task FileExistsAsync_returns_true()
        {
            var blobContainerClient = new Mock<BlobContainerClient>();
            var blobClient = new Mock<BlobClient>();
            var blobServiceClient = new Mock<BlobServiceClient>();
            var blobClientExtensionMock = new Mock<IBlobClientExtension>();

            blobServiceClient.Setup(x => x.GetBlobContainerClient(It.IsAny<string>())).Returns(blobContainerClient.Object);
            blobContainerClient.Setup(x => x.GetBlobClient(It.IsAny<string>())).Returns(blobClient.Object);
            blobClient.Setup(x => x.ExistsAsync(CancellationToken.None)).ReturnsAsync(Response.FromValue<bool>(true, null));

            var service = new VhAzureStorageService(blobServiceClient.Object, new WowzaConfiguration(), false, blobClientExtensionMock.Object);

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
            var blobClientExtensionMock = new Mock<IBlobClientExtension>();
            var blobServiceClient = new Mock<BlobServiceClient>();
            var service = new VhAzureStorageService(blobServiceClient.Object, config, false, blobClientExtensionMock.Object);

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
            var blobClientExtensionMock = new Mock<IBlobClientExtension>();

            blobServiceClient
                .Setup(x => x.GetUserDelegationKeyAsync(It.IsAny<DateTimeOffset>(), It.IsAny<DateTimeOffset>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Response.FromValue(BlobsModelFactory.UserDelegationKey("","","","","", DateTimeOffset.Now, DateTimeOffset.Now), null));
            
            var service = new VhAzureStorageService(blobServiceClient.Object, config, true, blobClientExtensionMock.Object);

            var result = await service.CreateSharedAccessSignature("myFilePath", TimeSpan.FromDays(7));

            result.Should().NotBeNullOrEmpty();
            result.Should().StartWith($"{config.StorageEndpoint}{config.StorageContainerName}/myFilePath?");

            blobServiceClient
                .Verify(x => x.GetUserDelegationKeyAsync(It.IsAny<DateTimeOffset>(), It.IsAny<DateTimeOffset>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task GetAllBlobsAsync_returns_blob_clients()
        {
            var config = new WowzaConfiguration
            {
                StorageEndpoint = "https://container.blob.core.windows.net/", StorageContainerName = "container",
                StorageAccountName = "accountName", StorageAccountKey = "YWNjb3VudEtleQ=="
            };

            var filePathPrefix = "myFilePath";
            var blobServiceClient = new Mock<BlobServiceClient>();
            var blobContainerClientMock = new Mock<BlobContainerClient>();
            var blobClientMock = new Mock<BlobClient>();
            var blobClientExtensionMock = new Mock<IBlobClientExtension>();

            blobServiceClient.Setup(x => x.GetBlobContainerClient(config.StorageContainerName)).Returns(blobContainerClientMock.Object);
            var pageable = new Mock<AsyncPageable<BlobItem>>();
            
            blobContainerClientMock.Setup(x => x.GetBlobsAsync(BlobTraits.None, BlobStates.None, filePathPrefix, default))
                .Returns(pageable.Object);
            pageable.Setup(x => x.GetAsyncEnumerator(default)).Returns(GetMockBlobItems());
            blobContainerClientMock.Setup(x => x.GetBlobClient(It.IsAny<string>())).Returns(blobClientMock.Object);
            
            var service = new VhAzureStorageService(blobServiceClient.Object, config, true, blobClientExtensionMock.Object);

            await foreach (var item in service.GetAllBlobsAsync(filePathPrefix))
            {
                item.Should().NotBeNull();
            }
        }

        [Test]
        public async Task GetAllBlobNamesByFilePathPrefix_returns_blob_file_names()
        {
            var config = new WowzaConfiguration
            {
                StorageEndpoint = "https://container.blob.core.windows.net/", StorageContainerName = "container",
                StorageAccountName = "accountName", StorageAccountKey = "YWNjb3VudEtleQ=="
            };

            var filePathPrefix = "myFilePath";
            var blobServiceClient = new Mock<BlobServiceClient>();
            var blobContainerClientMock = new Mock<BlobContainerClient>();
            var blobClientMock = new Mock<BlobClient>();
            var blobClientExtensionMock = new Mock<IBlobClientExtension>();

            blobClientMock.Setup(x => x.Name).Returns("SomeBlob.mp4");

            blobServiceClient.Setup(x => x.GetBlobContainerClient(config.StorageContainerName)).Returns(blobContainerClientMock.Object);
            var pageable = new Mock<AsyncPageable<BlobItem>>();

            blobContainerClientMock.Setup(x => x.GetBlobsAsync(BlobTraits.None, BlobStates.None, filePathPrefix, default))
                .Returns(pageable.Object);
            pageable.Setup(x => x.GetAsyncEnumerator(default)).Returns(GetMockBlobItems());
            blobContainerClientMock.Setup(x => x.GetBlobClient(It.IsAny<string>())).Returns(blobClientMock.Object);

            var service = new VhAzureStorageService(blobServiceClient.Object, config, true, blobClientExtensionMock.Object);

            var list = await service.GetAllBlobNamesByFilePathPrefix(filePathPrefix);
            list.Should().NotBeNull();
            list.Count().Should().Be(1);
        }

        /*
        
        [Test]
        public async Task GetEmptyBlobNamesByFilePathPrefix_returns_blob_file_names()
        {
            var config = new WowzaConfiguration
            {
                StorageEndpoint = "https://container.blob.core.windows.net/", StorageContainerName = "container",
                StorageAccountName = "accountName", StorageAccountKey = "YWNjb3VudEtleQ=="
            };

            var filePathPrefix = "myFilePath";
            var blobServiceClient = new Mock<BlobServiceClient>();
            var blobContainerClientMock = new Mock<BlobContainerClient>();
            var blobClientMock = new Mock<BlobClient>();
            var blobItemMock = new Mock<BlobItem>();
            var blobProperties = new BlobProperties();

            var blobClientExtensionMock = new Mock<IBlobClientExtension>();
            
            var pageable = new Mock<AsyncPageable<BlobItem>>();
            pageable.Setup(x => x.GetAsyncEnumerator(default)).Returns(GetMockBlobItems());

            blobContainerClientMock.Setup(x => x.GetBlobsAsync(BlobTraits.None, BlobStates.None, filePathPrefix, default)).Returns(pageable.Object);
            blobContainerClientMock.Setup(x => x.GetBlobClient(It.IsAny<string>())).Returns(blobClientMock.Object);


            blobServiceClient.Setup(x => x.GetBlobContainerClient(config.StorageContainerName)).Returns(blobContainerClientMock.Object);

            blobClientMock.Setup(x => x.Name).Returns("SomeBlob.mp4");

            blobClientExtensionMock.Setup(x => x.GetPropertiesAsync(It.IsAny<BlobClient>())).Returns(blobClientMock.Object.GetPropertiesAsync());
            
            var service = new VhAzureStorageService(blobServiceClient.Object,  config, true, blobClientExtensionMock.Object);

            var list = await service.GetAllEmptyBlobsByFilePathPrefix(filePathPrefix);

            list.Should().NotBeNull();
            list.Count().Should().Be(1);
        }
        

        [Test]
        public async Task Should_Check_Storage_For_Files_With_Prefix_And_Match_Count()
        {
            var guid1 = Guid.NewGuid();

            var results = new List<string>() { guid1.ToString() };

            var config = new WowzaConfiguration
            {
                StorageEndpoint = "https://container.blob.core.windows.net/", StorageContainerName = "container",
                StorageAccountName = "accountName", StorageAccountKey = "YWNjb3VudEtleQ=="
            };

            var blobServiceClient = new Mock<BlobServiceClient>();

            Mock<AzureStorageServiceBase> AzureStorageServiceMock = new Mock<AzureStorageServiceBase>();

            AzureStorageServiceMock.Setup(x => x.GetAllBlobNamesByFilePathPrefix(It.IsAny<string>())).ReturnsAsync(results);
            AzureStorageServiceMock.Setup(x => x.GetAllEmptyBlobsByFilePathPrefix(It.IsAny<string>())).ReturnsAsync(new List<string>());

            var service = new VhAzureStorageService(blobServiceClient.Object, config, true);

            var result = service.ReconcileFilesInStorage(guid1.ToString(), 1);

            AzureStorageServiceMock.Verify(x => x.GetAllBlobNamesByFilePathPrefix(It.IsAny<string>()), Times.Once);
            AzureStorageServiceMock.Verify(x => x.GetAllEmptyBlobsByFilePathPrefix(It.IsAny<string>()), Times.Once);

            result.Should().Be(true);

        }

        
            [Test]
            public async Task Should_Not_Throw_AudioPlatformFileNotFoundException_If_Storage_Returns_More_Files_With_Parameter_FileCount()
            {
                var guid1 = Guid.NewGuid();

                var results = new List<string>() { guid1.ToString(), guid1.ToString() + "_121" };

                AzureStorageServiceFactoryMock.Setup(x => x.Create(AzureStorageServiceType.Vh)).Returns(AzureStorageServiceMock.Object);
                AudioPlatformServiceMock.Reset();
                AzureStorageServiceMock.Reset();

                AzureStorageServiceMock.Setup(x => x.GetAllBlobNamesByFilePathPrefix(It.IsAny<string>())).ReturnsAsync(results);
                AzureStorageServiceMock.Setup(x => x.GetAllEmptyBlobsByFilePathPrefix(It.IsAny<string>())).ReturnsAsync(new List<string>());

                AudioFilesInStorageRequest request = new AudioFilesInStorageRequest() { FileNamePrefix = guid1.ToString(), FilesCount = 1 };

                var requestResponse = await Controller.ReconcileAudioFilesInStorage(request);

                AzureStorageServiceMock.Verify(x => x.GetAllBlobNamesByFilePathPrefix(It.IsAny<string>()), Times.Once);
                AzureStorageServiceMock.Verify(x => x.GetAllEmptyBlobsByFilePathPrefix(It.IsAny<string>()), Times.Once);

                var typedResult = (OkObjectResult)requestResponse;
                typedResult.StatusCode.Should().Be((int)HttpStatusCode.OK);

            }

            [Test]
            public void Should_Throw_AudioPlatformFileNotFoundException_If_Storage_Returns_Less_Files_With_Parameter_FileCount()
            {
                var guid1 = Guid.NewGuid();

                var msg = $"CheckAudioFilesInStorage - File name prefix :" + guid1.ToString() + "  Expected: " + "1" + " Actual:" + "0";


                AzureStorageServiceFactoryMock.Setup(x => x.Create(AzureStorageServiceType.Vh)).Returns(AzureStorageServiceMock.Object);
                AudioPlatformServiceMock.Reset();
                AzureStorageServiceMock.Reset();

                AzureStorageServiceMock.Setup(x => x.GetAllBlobNamesByFilePathPrefix(It.IsAny<string>())).ReturnsAsync(new List<string>());
                AzureStorageServiceMock.Setup(x => x.GetAllEmptyBlobsByFilePathPrefix(It.IsAny<string>())).ReturnsAsync(new List<string>());

                AudioFilesInStorageRequest request = new AudioFilesInStorageRequest() { FileNamePrefix = guid1.ToString(), FilesCount = 1 };

                Assert.That(async () => await Controller.ReconcileAudioFilesInStorage(request), Throws.TypeOf<AudioPlatformFileNotFoundException>().With.Message.EqualTo(msg));

                AzureStorageServiceMock.Verify(x => x.GetAllBlobNamesByFilePathPrefix(It.IsAny<string>()), Times.Once);
                AzureStorageServiceMock.Verify(x => x.GetAllEmptyBlobsByFilePathPrefix(It.IsAny<string>()), Times.Once);


            }

        [Test]
        public void Should_Throw_AudioPlatformFileNotFoundException_If_Storage_Returns_Any_Empty_Files()
        {
            var guid1 = Guid.NewGuid();

            var msg = $"CheckAudioFilesInStorage - File name prefix :" + guid1.ToString() + "  Expected: " + "1" + " Actual:" + "0";

            var results = new List<string>() { guid1.ToString() };

            AzureStorageServiceFactoryMock.Setup(x => x.Create(AzureStorageServiceType.Vh)).Returns(AzureStorageServiceMock.Object);
            AudioPlatformServiceMock.Reset();
            AzureStorageServiceMock.Reset();

            AzureStorageServiceMock.Setup(x => x.GetAllBlobNamesByFilePathPrefix(It.IsAny<string>())).ReturnsAsync(results);
            AzureStorageServiceMock.Setup(x => x.GetAllEmptyBlobsByFilePathPrefix(It.IsAny<string>())).ReturnsAsync(results);

            AudioFilesInStorageRequest request = new AudioFilesInStorageRequest() { FileNamePrefix = guid1.ToString(), FilesCount = 1 };

            Assert.That(async () => await Controller.ReconcileAudioFilesInStorage(request), Throws.TypeOf<AudioPlatformFileNotFoundException>().With.Message.EqualTo(msg));

            AzureStorageServiceMock.Verify(x => x.GetAllBlobNamesByFilePathPrefix(It.IsAny<string>()), Times.Once);
            AzureStorageServiceMock.Verify(x => x.GetAllEmptyBlobsByFilePathPrefix(It.IsAny<string>()), Times.Once);

        }
     */




        private async IAsyncEnumerator<BlobItem> GetMockBlobItems()
        {
            var blobItem = new Mock<BlobItem>();

            yield return blobItem.Object;

            await Task.CompletedTask;
        }
    }
}
