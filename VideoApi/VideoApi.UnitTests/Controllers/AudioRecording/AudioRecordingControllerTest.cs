using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Autofac.Extras.Moq;
using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Testing.Common.Helper.Builders.Domain;
using VideoApi.Contract.Requests;
using VideoApi.Contract.Responses;
using VideoApi.Controllers;
using VideoApi.DAL.Queries;
using VideoApi.DAL.Queries.Core;
using VideoApi.Domain.Enums;
using VideoApi.Services.Contracts;
using VideoApi.Services.Exceptions;
using VideoApi.Services.Factories;

namespace VideoApi.UnitTests.Controllers.AudioRecording
{
    [TestFixture]
    public class AudioRecordingControllerTest
    {
        [SetUp]
        public void Setup()
        {
            _mocker = AutoMock.GetLoose();
            _queryHandler         = _mocker.Mock<IQueryHandler>();
            _storageServiceFactory = _mocker.Mock<IAzureStorageServiceFactory>();
            _storageService        = _mocker.Mock<IAzureStorageService>();

            _testConference = new ConferenceBuilder()
                             .WithParticipant(UserRole.Judge, null)
                             .WithParticipant(UserRole.Individual, "Applicant", null, null, RoomType.ConsultationRoom)
                             .WithParticipant(UserRole.Representative, "Applicant")
                             .WithParticipant(UserRole.Individual, "Respondent")
                             .WithParticipant(UserRole.Representative, "Respondent")
                             .WithAudioRecordingRequired(true)
                             .Build();

            _queryHandler
               .Setup(x =>
                          x.Handle<GetConferenceByHearingRefIdQuery, VideoApi.Domain.Conference>(
                           It.IsAny<GetConferenceByHearingRefIdQuery>()))
               .ReturnsAsync(_testConference);
            
            _audioPlatformServiceMock = _mocker.Mock<IAudioPlatformService>();
            _azureStorageServiceFactoryMock = _mocker.Mock<IAzureStorageServiceFactory>();
            _azureStorageServiceMock = _mocker.Mock<IAzureStorageService>();
            _mocker.Mock<ILogger<AudioRecordingController>>();
            _controller = _mocker.Create<AudioRecordingController>();
        }
        
        private Mock<IAzureStorageServiceFactory> _storageServiceFactory;
        private Mock<IAzureStorageService> _storageService;
        private Mock<IQueryHandler> _queryHandler;
        private VideoApi.Domain.Conference _testConference;
        
        private AudioRecordingController _controller;
        private Mock<BlobClient> _blobClientMock;
        private AutoMock _mocker;
        private Mock<IAzureStorageServiceFactory> _azureStorageServiceFactoryMock;
        private Mock<IAzureStorageService> _azureStorageServiceMock;
        private Mock<IAudioPlatformService> _audioPlatformServiceMock;
        
        [Test]
        public async Task GetAudioRecordingLinkAsync_returns_audio_file_link()
        {
            var hearingId = Guid.NewGuid().ToString();
            var filePath = $"{hearingId}_2020-01-01.mp4";
            
            _storageServiceFactory.Setup(x => x.Create(AzureStorageServiceType.Vh)).Returns(_storageService.Object);
           
            var blobFileNames = new List<string> { filePath };
            _storageService.Setup(x => x.GetAllBlobNamesByFilePathPrefix(It.IsAny<string>())).ReturnsAsync(blobFileNames);
            _storageService
                .Setup(x => x.CreateSharedAccessSignature(filePath, It.IsAny<TimeSpan>()))
                .ReturnsAsync("fileLink");

            
            var result = await _controller.GetAudioRecordingLinkAsync(hearingId) as OkObjectResult;
            result.Should().NotBeNull();
            result.StatusCode.Should().Be(StatusCodes.Status200OK);
            var item = result.Value.As<AudioRecordingResponse>();
            item.Should().NotBeNull();
            item.AudioFileLinks.Should().NotBeNullOrEmpty();
            item.AudioFileLinks.First().Should().Be("fileLink");
            _storageService.Verify(c=>c.CreateSharedAccessSignature(filePath, It.IsAny<TimeSpan>()));
        }
        
        [Test]
        public async Task GetAudioRecordingLinkCvpAllWithCaseReferenceAsync_returns_ok_with_results()
        {
            const string cloudRoom = "001";
            const string date = "2020-09-01";
            const string caseReference = "case123";
            
            _blobClientMock = new Mock<BlobClient>();
            var blobName = $"{caseReference}someBlobName{date}";
            var blobFullName = $"{cloudRoom}/{blobName}";
            _blobClientMock.Setup(x => x.Name).Returns(blobFullName);
            
            _storageServiceFactory.Setup(x => x.Create(AzureStorageServiceType.Cvp)).Returns(_storageService.Object);
            _storageService.Setup(x => x.GetAllBlobsAsync($"audiostream{cloudRoom}")).Returns(GetMockBlobClients);
            _storageService.Setup(x => x.CreateSharedAccessSignature(blobFullName, It.IsAny<TimeSpan>())).ReturnsAsync("sas");

            var result = await _controller.GetAudioRecordingLinkCvpAllAsync(cloudRoom, date, caseReference) as OkObjectResult;
            result.Should().NotBeNull();
            result.StatusCode.Should().Be(StatusCodes.Status200OK);
            var item = result.Value.As<List<CvpAudioFileResponse>>();
            item.Should().NotBeNullOrEmpty();
            var firstItem = item.FirstOrDefault();
            firstItem.Should().NotBeNull();
            firstItem.FileName.Should().Be(blobName);
            firstItem.SasTokenUrl.Should().Be("sas");
            
            _storageService.Verify(x => x.CreateSharedAccessSignature(blobFullName, It.IsAny<TimeSpan>()), Times.Once);
        }
        
        [Test]
        public async Task GetAudioRecordingLinkCvpAllWithCaseReferenceAsync_returns_ok_with_no_results()
        {
            const string cloudRoom = "001";
            const string date = "2020-09-01";
            const string caseReference = "case123";

            _blobClientMock = new Mock<BlobClient>();
            var blobName = "TotallyNonMatchingNameWIthSearchParameters";
            var blobFullName = $"{cloudRoom}/{blobName}";
            _blobClientMock.Setup(x => x.Name).Returns(blobFullName);

            _storageServiceFactory.Setup(x => x.Create(AzureStorageServiceType.Cvp)).Returns(_storageService.Object);
            _storageService.Setup(x => x.GetAllBlobsAsync($"audiostream{cloudRoom}")).Returns(GetMockBlobClients);
            _storageService.Setup(x => x.CreateSharedAccessSignature(blobFullName, It.IsAny<TimeSpan>())).ReturnsAsync("sas");

            var result = await _controller.GetAudioRecordingLinkCvpAllAsync(cloudRoom, date, caseReference) as OkObjectResult;
            result.Should().NotBeNull();
            result.StatusCode.Should().Be(StatusCodes.Status200OK);
            var item = result.Value.As<List<CvpAudioFileResponse>>();
            item.Should().BeNullOrEmpty();

            _storageService.Verify(x => x.CreateSharedAccessSignature(blobFullName, It.IsAny<TimeSpan>()), Times.Never);
        }
        
        [Test]
        public async Task GetAudioRecordingLinkCvpAllWithCaseReferenceAsync_returns_not_found_when_exception_thrown()
        {
            const string cloudRoom = "001";
            const string date = "2020-09-01";
            const string caseReference = "case123";

            _blobClientMock = new Mock<BlobClient>();
            var blobName = $"TotallyNonMatchingNameWIthSearchParameters";
            var blobFullName = $"{cloudRoom}/{blobName}";
            _blobClientMock.Setup(x => x.Name).Returns(blobFullName);

            _storageServiceFactory.Setup(x => x.Create(AzureStorageServiceType.Cvp)).Returns(_storageService.Object);
            _storageService.Setup(x => x.GetAllBlobsAsync($"audiostream{cloudRoom}")).Throws<Exception>();

            var result = await _controller.GetAudioRecordingLinkCvpAllAsync(cloudRoom, date, caseReference) as NotFoundResult;
            result.Should().NotBeNull();
            result.StatusCode.Should().Be(StatusCodes.Status404NotFound);

            _storageService.Verify(x => x.CreateSharedAccessSignature(blobFullName, It.IsAny<TimeSpan>()), Times.Never);
        }
        
        [Test]
        public async Task GetAudioRecordingLinkCvpByCloudRoomAsync_returns_ok_with_results()
        {
            const string cloudRoom = "001";
            const string date = "2020-09-01";
            
            _blobClientMock = new Mock<BlobClient>();
            var blobName = $"someBlobName{date}";
            var blobFullName = $"{cloudRoom}/{blobName}";
            _blobClientMock.Setup(x => x.Name).Returns(blobFullName);
            
            _storageServiceFactory.Setup(x => x.Create(AzureStorageServiceType.Cvp)).Returns(_storageService.Object);
            _storageService.Setup(x => x.GetAllBlobsAsync($"audiostream{cloudRoom}")).Returns(GetMockBlobClients);
            _storageService.Setup(x => x.CreateSharedAccessSignature(blobFullName, It.IsAny<TimeSpan>())).ReturnsAsync("sas");

            var result = await _controller.GetAudioRecordingLinkCvpByCloudRoomAsync(cloudRoom, date) as OkObjectResult;
            result.Should().NotBeNull();
            result.StatusCode.Should().Be(StatusCodes.Status200OK);
            var item = result.Value.As<List<CvpAudioFileResponse>>();
            item.Should().NotBeNullOrEmpty();
            var firstItem = item.FirstOrDefault();
            firstItem.Should().NotBeNull();
            firstItem.FileName.Should().Be(blobName);
            firstItem.SasTokenUrl.Should().Be("sas");
            
            _storageService.Verify(x => x.CreateSharedAccessSignature(blobFullName, It.IsAny<TimeSpan>()), Times.Once);
        }
        
        [Test]
        public async Task GetAudioRecordingLinkCvpByCloudRoomAsync_returns_ok_with_no_results()
        {
            const string cloudRoom = "001";
            const string date = "2020-09-01";

            _blobClientMock = new Mock<BlobClient>();
            var blobName = "TotallyNonMatchingNameWIthSearchParameters";
            var blobFullName = $"audiostream{cloudRoom}/{blobName}";
            _blobClientMock.Setup(x => x.Name).Returns(blobFullName);

            _storageServiceFactory.Setup(x => x.Create(AzureStorageServiceType.Cvp)).Returns(_storageService.Object);
            _storageService.Setup(x => x.GetAllBlobsAsync($"audiostream{cloudRoom}")).Returns(GetMockBlobClients);
            _storageService.Setup(x => x.CreateSharedAccessSignature(blobFullName, It.IsAny<TimeSpan>())).ReturnsAsync("sas");

            var result = await _controller.GetAudioRecordingLinkCvpByCloudRoomAsync(cloudRoom, date) as OkObjectResult;
            result.Should().NotBeNull();
            result.StatusCode.Should().Be(StatusCodes.Status200OK);
            var item = result.Value.As<List<CvpAudioFileResponse>>();
            item.Should().BeNullOrEmpty();

            _storageService.Verify(x => x.CreateSharedAccessSignature(blobFullName, It.IsAny<TimeSpan>()), Times.Never);
        }
        
        [Test]
        public async Task GetAudioRecordingLinkCvpByCloudRoomAsync_returns_not_found_when_exception_thrown()
        {
            const string cloudRoom = "audiostream001";
            const string date = "2020-09-01";

            _blobClientMock = new Mock<BlobClient>();
            var blobName = $"TotallyNonMatchingNameWIthSearchParameters";
            var blobFullName = $"{cloudRoom}/{blobName}";
            _blobClientMock.Setup(x => x.Name).Returns(blobFullName);

            _storageServiceFactory.Setup(x => x.Create(AzureStorageServiceType.Cvp)).Returns(_storageService.Object);
            _storageService.Setup(x => x.GetAllBlobsAsync(cloudRoom)).Throws<Exception>();

            var result = await _controller.GetAudioRecordingLinkCvpByCloudRoomAsync(cloudRoom, date) as NotFoundResult;
            result.Should().NotBeNull();
            result.StatusCode.Should().Be(StatusCodes.Status404NotFound);

            _storageService.Verify(x => x.CreateSharedAccessSignature(blobFullName, It.IsAny<TimeSpan>()), Times.Never);
        }
        
        [Test]
        public async Task GetAudioRecordingLinkCvpByDateAsync_returns_ok_with_results()
        {
            const string date = "2020-09-01";
            
            _blobClientMock = new Mock<BlobClient>();
            var blobName = $"caseNumber-someBlobName-{date}";
            var blobFullName = $"someCloudRoom/{blobName}";
            _blobClientMock.Setup(x => x.Name).Returns(blobFullName);
            
            _storageServiceFactory.Setup(x => x.Create(AzureStorageServiceType.Cvp)).Returns(_storageService.Object);
            _storageService.Setup(x => x.GetAllBlobsAsync(string.Empty)).Returns(GetMockBlobClients);
            _storageService.Setup(x => x.CreateSharedAccessSignature(blobFullName, It.IsAny<TimeSpan>())).ReturnsAsync("sas");

            var result = await _controller.GetAudioRecordingLinkCvpByDateAsync(date, "caseNumber") as OkObjectResult;
            result.Should().NotBeNull();
            result.StatusCode.Should().Be(StatusCodes.Status200OK);
            var item = result.Value.As<List<CvpAudioFileResponse>>();
            item.Should().NotBeNullOrEmpty();
            var firstItem = item.FirstOrDefault();
            firstItem.Should().NotBeNull();
            firstItem.FileName.Should().Be(blobName);
            firstItem.SasTokenUrl.Should().Be("sas");
            
            _storageService.Verify(x => x.CreateSharedAccessSignature(blobFullName, It.IsAny<TimeSpan>()), Times.Once);
        }
        
        [Test]
        public async Task GetAudioRecordingLinkCvpByDateAsync_returns_ok_with_no_results()
        {
            const string cloudRoom = "001";
            const string date = "2020-09-01";

            _blobClientMock = new Mock<BlobClient>();
            var blobName = "TotallyNonMatchingNameWIthSearchParameters";
            var blobFullName = $"audiostream{cloudRoom}/{blobName}";
            _blobClientMock.Setup(x => x.Name).Returns(blobFullName);

            _storageServiceFactory.Setup(x => x.Create(AzureStorageServiceType.Cvp)).Returns(_storageService.Object);
            _storageService.Setup(x => x.GetAllBlobsAsync(string.Empty)).Returns(GetMockBlobClients);
            _storageService.Setup(x => x.CreateSharedAccessSignature(blobFullName, It.IsAny<TimeSpan>())).ReturnsAsync("sas");

            var result = await _controller.GetAudioRecordingLinkCvpByDateAsync(date, "someCaseReference") as OkObjectResult;
            result.Should().NotBeNull();
            result.StatusCode.Should().Be(StatusCodes.Status200OK);
            var item = result.Value.As<List<CvpAudioFileResponse>>();
            item.Should().BeNullOrEmpty();

            _storageService.Verify(x => x.CreateSharedAccessSignature(blobFullName, It.IsAny<TimeSpan>()), Times.Never);
        }
        
        [Test]
        public async Task GetAudioRecordingLinkCvpByDateAsync_returns_not_found_when_exception_thrown()
        {
            const string cloudRoom = "audiostream001";
            const string date = "2020-09-01";

            _blobClientMock = new Mock<BlobClient>();
            var blobName = "TotallyNonMatchingNameWIthSearchParameters";
            var blobFullName = $"{cloudRoom}/{blobName}";
            _blobClientMock.Setup(x => x.Name).Returns(blobFullName);

            _storageServiceFactory.Setup(x => x.Create(AzureStorageServiceType.Cvp)).Returns(_storageService.Object);
            _storageService.Setup(x => x.GetAllBlobsAsync(string.Empty)).Throws<Exception>();

            var result = await _controller.GetAudioRecordingLinkCvpByDateAsync(date, "caseReference") as NotFoundResult;
            result.Should().NotBeNull();
            result.StatusCode.Should().Be(StatusCodes.Status404NotFound);

            _storageService.Verify(x => x.CreateSharedAccessSignature(blobFullName, It.IsAny<TimeSpan>()), Times.Never);
        }
        
        [Test]
        public async Task ReconcileAudioFilesInStorage_Should_Check_Storage_For_Files_With_Prefix_And_Match_Count()
        {
            var guid1 = Guid.NewGuid();

            _azureStorageServiceFactoryMock.Setup(x => x.Create(AzureStorageServiceType.Vh)).Returns(_azureStorageServiceMock.Object);
            _audioPlatformServiceMock.Reset();
            _azureStorageServiceMock.Reset();

            _azureStorageServiceMock.Setup(x => x.ReconcileFilesInStorage(It.IsAny<string>(), It.IsAny<int>())).ReturnsAsync(true);

            AudioFilesInStorageRequest request = new AudioFilesInStorageRequest() { FileNamePrefix = guid1.ToString(), FilesCount = 1 };

            var requestResponse = await _controller.ReconcileAudioFilesInStorage(request);

            _azureStorageServiceMock.Verify(x => x.ReconcileFilesInStorage(It.IsAny<string>(), It.IsAny<int>()), Times.Once);

            var typedResult = (OkObjectResult)requestResponse;
            typedResult.StatusCode.Should().Be((int)HttpStatusCode.OK);

        }

        [Test]
        public void ReconcileAudioFilesInStorage_Should_Throw_Exception_With_Null_Request()
        {
            _azureStorageServiceFactoryMock.Setup(x => x.Create(AzureStorageServiceType.Vh)).Returns(_azureStorageServiceMock.Object);
            _audioPlatformServiceMock.Reset();
            _azureStorageServiceMock.Reset();

            _azureStorageServiceMock.Setup(x => x.ReconcileFilesInStorage(It.IsAny<string>(), It.IsAny<int>())).ReturnsAsync(true);

            AudioFilesInStorageRequest request = null;
            var msg = $"ReconcileFilesInStorage - File Name prefix is required.";

            Assert.That(async () => await _controller.ReconcileAudioFilesInStorage(request), Throws.TypeOf<AudioPlatformFileNotFoundException>().With.Message.EqualTo(msg));

            _azureStorageServiceMock.Verify(x => x.ReconcileFilesInStorage(It.IsAny<string>(), It.IsAny<int>()), Times.Never);

        }

        [Test]
        public void ReconcileAudioFilesInStorage_Should_Throw_Exception_With_Request_File_Count_Is_Zero()
        {
            _azureStorageServiceFactoryMock.Setup(x => x.Create(AzureStorageServiceType.Vh)).Returns(_azureStorageServiceMock.Object);
            _audioPlatformServiceMock.Reset();
            _azureStorageServiceMock.Reset();

            _azureStorageServiceMock.Setup(x => x.ReconcileFilesInStorage(It.IsAny<string>(), It.IsAny<int>())).ReturnsAsync(true);

            AudioFilesInStorageRequest request = new AudioFilesInStorageRequest() {FileNamePrefix="Prefilx", FilesCount = 0 };
            var msg = $"ReconcileFilesInStorage - File count cannot be negative or zero.";

            Assert.That(async () => await _controller.ReconcileAudioFilesInStorage(request), Throws.TypeOf<AudioPlatformFileNotFoundException>().With.Message.EqualTo(msg));

            _azureStorageServiceMock.Verify(x => x.ReconcileFilesInStorage(It.IsAny<string>(), It.IsAny<int>()), Times.Never);

        }

        [Test]
        public void ReconcileAudioFilesInStorage_Should_Catch_Exception_When_Service_Throws_Exception()
        {
            var guid1 = Guid.NewGuid();

            _azureStorageServiceFactoryMock.Setup(x => x.Create(AzureStorageServiceType.Vh)).Returns(_azureStorageServiceMock.Object);

            _audioPlatformServiceMock.Reset();
            _azureStorageServiceMock.Reset();

            _azureStorageServiceMock.Setup(x => x.ReconcileFilesInStorage(It.IsAny<string>(), It.IsAny<int>())).Throws(new AudioPlatformFileNotFoundException(It.IsAny<string>(), HttpStatusCode.NotFound));

            AudioFilesInStorageRequest request = new AudioFilesInStorageRequest() { FileNamePrefix = guid1.ToString(), FilesCount = 1 };
                            
            Assert.That(async () => await _controller.ReconcileAudioFilesInStorage(request), Throws.TypeOf<AudioPlatformFileNotFoundException>());

            _azureStorageServiceMock.Verify(x => x.ReconcileFilesInStorage(It.IsAny<string>(), It.IsAny<int>()), Times.Once);

        }
        
        private async IAsyncEnumerable<BlobClient> GetMockBlobClients()
        {
            yield return _blobClientMock.Object;

            await Task.CompletedTask;
        }
    }
}
