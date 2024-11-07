using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Moq;
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
        private Mock<BlobContainerClient> _blobContainerClient;
        private Mock<BlobClient>  _blobClientMock ;
        private Mock<BlobServiceClient> _blobServiceClient ;
        private Mock<IBlobClientExtension> _blobClientExtensionMock;
        private VhAzureStorageService _service;
        private WowzaConfiguration _config;
        private string _filePathPrefix;
        private Mock<BlobContainerClient> _blobContainerClientMock ;
        private BlobProperties _emptyBlobClientProperties;
        private BlobProperties _notEmptyBlobClientProperties;
        private Mock<AsyncPageable<BlobItem>> _pageable;

        [SetUp]
        public void Setup()
        {
            _blobContainerClient = new Mock<BlobContainerClient>();
            _blobClientMock = new Mock<BlobClient>();
            _blobServiceClient = new Mock<BlobServiceClient>();
            _blobClientExtensionMock = new Mock<IBlobClientExtension>();
            _blobContainerClientMock = new Mock<BlobContainerClient>();
            _pageable = new Mock<AsyncPageable<BlobItem>>();
            _filePathPrefix = "myFilePath";

            _config = new WowzaConfiguration
            {
                StorageEndpoint = "https://container.blob.core.windows.net/", StorageContainerName = "container",
                StorageAccountName = "accountName", StorageAccountKey = "YWNjb3VudEtleQ=="
            };

            _service = new VhAzureStorageService(_blobServiceClient.Object,_config, true, _blobClientExtensionMock.Object);

            _emptyBlobClientProperties = BlobsModelFactory.BlobProperties(DateTimeOffset.UtcNow,
                leaseState: LeaseState.Available,
                leaseStatus: LeaseStatus.Unlocked,
                contentLength: 0,
                LeaseDurationType.Infinite, ETag.All, new byte[] { 1 }, "", "", "", "", "",
                false, 121, CopyStatus.Success, "", new Uri("https://www.portal.azure.com"), 0, "", false, "", "", "", "",
                DateTimeOffset.UtcNow, "", BlobType.Block, false, new Dictionary<string, 
                string>(), "", DateTimeOffset.UtcNow, DateTime.UtcNow, "");

             _notEmptyBlobClientProperties = BlobsModelFactory.BlobProperties(DateTimeOffset.UtcNow,
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
            
            _blobServiceClient.Setup(x => x.GetBlobContainerClient(It.IsAny<string>())).Returns(_blobContainerClient.Object);
            _blobContainerClient.Setup(x => x.GetBlobClient(It.IsAny<string>())).Returns(_blobClientMock.Object);
            _blobClientMock.Setup(x => x.ExistsAsync(CancellationToken.None)).ReturnsAsync(Response.FromValue<bool>(true, null));
                        
            var result = await _service.FileExistsAsync(It.IsAny<string>());

            result.Should().BeTrue();
        }
        
        [Test]
        public async Task CreateSharedAccessSignature_returns_token_created_using_StorageSharedKeyCredential()
        {
            var service = new VhAzureStorageService(_blobServiceClient.Object, _config, false, _blobClientExtensionMock.Object);

            var result = await service.CreateSharedAccessSignature("myFilePath", It.IsAny<TimeSpan>());

            result.Should().NotBeNullOrEmpty();
            result.Should().StartWith($"{_config.StorageEndpoint}{_config.StorageContainerName}/myFilePath?");

            _blobServiceClient
                .Verify(x => x.GetUserDelegationKeyAsync(It.IsAny<DateTimeOffset>(), It.IsAny<DateTimeOffset>(), It.IsAny<CancellationToken>()), Times.Never);
        }
        
        [Test]
        public async Task CreateSharedAccessSignature_returns_token_created_using_DelegationKey()
        {
            
            _blobServiceClient
                .Setup(x => x.GetUserDelegationKeyAsync(It.IsAny<DateTimeOffset>(), It.IsAny<DateTimeOffset>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Response.FromValue(BlobsModelFactory.UserDelegationKey("","","","","", DateTimeOffset.Now, DateTimeOffset.Now), null));
            
            var result = await _service.CreateSharedAccessSignature("myFilePath", TimeSpan.FromDays(7));

            result.Should().NotBeNullOrEmpty();
            result.Should().StartWith($"{_config.StorageEndpoint}{_config.StorageContainerName}/myFilePath?");

            _blobServiceClient
                .Verify(x => x.GetUserDelegationKeyAsync(It.IsAny<DateTimeOffset>(), It.IsAny<DateTimeOffset>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task GetAllBlobsAsync_returns_blob_clients()
        {
            _blobServiceClient.Setup(x => x.GetBlobContainerClient(_config.StorageContainerName)).Returns(_blobContainerClientMock.Object);
            
            _blobContainerClientMock.Setup(x => x.GetBlobsAsync(BlobTraits.None, BlobStates.None, _filePathPrefix, default))
                .Returns(_pageable.Object);
            _pageable.Setup(x => x.GetAsyncEnumerator(default)).Returns(GetMockBlobItems());
            _blobContainerClientMock.Setup(x => x.GetBlobClient(It.IsAny<string>())).Returns(_blobClientMock.Object);
            
            await foreach (var item in _service.GetAllBlobsAsync(_filePathPrefix))
            {
                item.Should().NotBeNull();
            }
        }

        [Test]
        public async Task GetAllBlobNamesByFilePathPrefix_returns_blob_file_names()
        {
            _blobClientMock.Setup(x => x.Name).Returns("SomeBlob.mp4");

            _blobServiceClient.Setup(x => x.GetBlobContainerClient(_config.StorageContainerName)).Returns(_blobContainerClientMock.Object);
            
            _blobContainerClientMock.Setup(x => x.GetBlobsAsync(BlobTraits.None, BlobStates.None, _filePathPrefix, default))
                .Returns(_pageable.Object);
            _pageable.Setup(x => x.GetAsyncEnumerator(default)).Returns(GetMockBlobItems());
            _blobContainerClientMock.Setup(x => x.GetBlobClient(It.IsAny<string>())).Returns(_blobClientMock.Object);
            
            var list = await _service.GetAllBlobNamesByFilePathPrefix(_filePathPrefix);
            list.Should().NotBeNull();
            list.Count().Should().Be(1);
        }
        
        [Test]
        public async Task GetEmptyBlobNamesByFilePathPrefix_returns_blob_file_names_if_blob_content_length_is_zero()
        {
            _pageable.Setup(x => x.GetAsyncEnumerator(default)).Returns(GetMockBlobItems());

            _blobContainerClientMock.Setup(x => x.GetBlobsAsync(BlobTraits.None, BlobStates.None, _filePathPrefix, default)).Returns(_pageable.Object);
            _blobContainerClientMock.Setup(x => x.GetBlobClient(It.IsAny<string>())).Returns(_blobClientMock.Object);

            _blobServiceClient.Setup(x => x.GetBlobContainerClient(_config.StorageContainerName)).Returns(_blobContainerClientMock.Object);
            
            _blobClientExtensionMock.Setup(x => x.GetPropertiesAsync(It.IsAny<BlobClient>())).ReturnsAsync(_emptyBlobClientProperties);
            _blobClientMock.Setup(x => x.Name).Returns("SomeBlob.mp4");
            
            var list = await _service.GetAllEmptyBlobsByFilePathPrefix(_filePathPrefix);

            list.Should().NotBeNull();
            list.Count().Should().Be(1);
        }

        [Test]
        public async Task GetEmptyBlobNamesByFilePathPrefix_returns_empty_blob_file_names_if_blob_content_length_is__not_zero()
        {
            _pageable.Setup(x => x.GetAsyncEnumerator(default)).Returns(GetMockBlobItems());

            _blobContainerClientMock.Setup(x => x.GetBlobsAsync(BlobTraits.None, BlobStates.None, _filePathPrefix, default)).Returns(_pageable.Object);
            _blobContainerClientMock.Setup(x => x.GetBlobClient(It.IsAny<string>())).Returns(_blobClientMock.Object);

            _blobServiceClient.Setup(x => x.GetBlobContainerClient(_config.StorageContainerName)).Returns(_blobContainerClientMock.Object);

            _blobClientExtensionMock.Setup(x => x.GetPropertiesAsync(It.IsAny<BlobClient>())).ReturnsAsync(_notEmptyBlobClientProperties);
            _blobClientMock.Setup(x => x.Name).Returns("SomeBlob.mp4");

            var result = await _service.GetAllEmptyBlobsByFilePathPrefix(_filePathPrefix);

            result.Should().BeEmpty();
            result.Count().Should().Be(0);
        }


        [Test]
        public async Task Should_Check_Storage_For_Files_With_Prefix_And_Match_Count()
        {           
            _pageable.Setup(x => x.GetAsyncEnumerator(default)).Returns(GetMockBlobItems());

            _blobContainerClientMock.Setup(x => x.GetBlobsAsync(BlobTraits.None, BlobStates.None, _filePathPrefix, default)).Returns(_pageable.Object);
            _blobContainerClientMock.Setup(x => x.GetBlobClient(It.IsAny<string>())).Returns(_blobClientMock.Object);

            _blobClientExtensionMock.Setup(x => x.GetPropertiesAsync(It.IsAny<BlobClient>())).ReturnsAsync(_notEmptyBlobClientProperties);

            _blobServiceClient.Setup(x => x.GetBlobContainerClient(_config.StorageContainerName)).Returns(_blobContainerClientMock.Object);

            _blobClientMock.Setup(x => x.Name).Returns("SomeBlob.mp4");
                        
            var result = await _service.ReconcileFilesInStorage("myFilePath", 1);

            result.Should().Be(true);
        }

        [Test]
        public void Should_Throw_AudioPlatformFileNotFoundException_If_Storage_Returns_Less_Files_With_Parameter_FileCount()
        {
            _pageable.Setup(x => x.GetAsyncEnumerator(default)).Returns(GetEmptyMockBlobItems());

            _blobContainerClientMock.Setup(x => x.GetBlobsAsync(BlobTraits.None, BlobStates.None, _filePathPrefix, default)).Returns(_pageable.Object);

            _blobContainerClientMock.Setup(x => x.GetBlobClient(It.IsAny<string>())).Returns(_blobClientMock.Object);

            _blobClientExtensionMock.Setup(x => x.GetPropertiesAsync(It.IsAny<BlobClient>())).ReturnsAsync(_notEmptyBlobClientProperties);

            _blobServiceClient.Setup(x => x.GetBlobContainerClient(_config.StorageContainerName)).Returns(_blobContainerClientMock.Object);

            _blobClientMock.Setup(x => x.Name).Returns(_filePathPrefix+ ".mp4");

            var msg = $"ReconcileFilesInStorage - File name prefix :" + _filePathPrefix + "  Expected: " + "1" + " Actual:" + "0";

            Assert.That(async () => await _service.ReconcileFilesInStorage("myFilePath", 1), Throws.TypeOf<AudioPlatformFileNotFoundException>().With.Message.EqualTo(msg));

        }

        private static async IAsyncEnumerator<BlobItem> GetMockBlobItems()
        {
            var blobItem = new Mock<BlobItem>();

            yield return blobItem.Object;

            await Task.CompletedTask;
        }
       
        private static async IAsyncEnumerator<BlobItem> GetEmptyMockBlobItems()
        {
            await Task.CompletedTask;
            yield break;

        }
    }
}
