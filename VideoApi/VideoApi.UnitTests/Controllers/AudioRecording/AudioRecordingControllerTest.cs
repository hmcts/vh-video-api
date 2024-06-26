using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Testing.Common.Helper.Builders.Domain;
using VideoApi.Contract.Responses;
using VideoApi.Controllers;
using VideoApi.DAL.Queries;
using VideoApi.DAL.Queries.Core;
using VideoApi.Domain.Enums;
using VideoApi.Services.Factories;
using VideoApi.Services.Contracts;
using VideoApi.Services.Responses;

namespace VideoApi.UnitTests.Controllers.AudioRecording
{
    [TestFixture]
    public class AudioRecordingControllerTest
    {
        private Mock<IAudioPlatformService> _audioPlatformService;
        private Mock<IAzureStorageServiceFactory> _storageServiceFactory;
        private Mock<IAzureStorageService> _storageService;
        private Mock<IQueryHandler> _queryHandler;
        private VideoApi.Domain.Conference _testConference;

        private AudioRecordingController _controller;
        private Mock<BlobClient> _blobClientMock;

        private const string ApplicationName = "vh-recording-app";

        [SetUp]
        public void Setup()
        {
            _queryHandler         = new Mock<IQueryHandler>();
            _audioPlatformService = new Mock<IAudioPlatformService>();
            _audioPlatformService.Setup(x => x.ApplicationName).Returns(ApplicationName);
            _storageServiceFactory = new Mock<IAzureStorageServiceFactory>();
            _storageService        = new Mock<IAzureStorageService>();

            _controller = new AudioRecordingController
                (
                 _storageServiceFactory.Object, _audioPlatformService.Object,
                 new Mock<ILogger<AudioRecordingController>>().Object, _queryHandler.Object
                );

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
        }

        [Test]
        public async Task GetAudioStreamInfoAsync_Returns_NotFound()
        {
            _audioPlatformService
               .Setup(x => x.GetAudioStreamInfoAsync(It.IsAny<string>()))
               .Throws<AggregateException>();

            var result = await _controller.GetAudioStreamInfoAsync(It.IsAny<Guid>()) as NotFoundObjectResult;
            result.Should().NotBeNull();
            result.StatusCode.Should().Be(StatusCodes.Status404NotFound);
        }

        [TestCase]
        public async Task GetAudioStreamInfoAsync_Returns_AudioStreamInfoResponse()
        {
            var hearingId = Guid.NewGuid();
            var wowzaResponse = new WowzaGetStreamRecorderResponse
            {
                Option              = "Option",
                ApplicationName     = "ApplicationName",
                BaseFile            = "BaseFile",
                CurrentDuration     = 1,
                CurrentFile         = "CurrentFile",
                CurrentSize         = 1,
                FileFormat          = "FileFormat",
                InstanceName        = "InstanceName",
                OutputPath          = "OutputPath",
                RecorderName        = "RecorderName",
                RecorderState       = "RecorderState",
                SegmentDuration     = 1,
                ServerName          = "ServerName",
                RecorderErrorString = "RecorderErrorString",
                RecordingStartTime  = "RecordingStartTime"
            };

            _audioPlatformService
               .Setup(x => x.GetAudioStreamInfoAsync(It.IsAny<string>()))
               .ReturnsAsync(wowzaResponse);

            var result = await _controller.GetAudioStreamInfoAsync(hearingId) as OkObjectResult;
            result.Should().NotBeNull();
            result.StatusCode.Should().Be(StatusCodes.Status200OK);
            var response = result.Value as AudioStreamInfoResponse;
            response.Should().NotBeNull();
            response.Should().BeEquivalentTo(wowzaResponse, options => options.ExcludingMissingMembers());
            _audioPlatformService.Verify(e => e.GetAudioStreamInfoAsync(hearingId.ToString()), Times.Once);
        }
        
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

        private async IAsyncEnumerable<BlobClient> GetMockBlobClients()
        {
            yield return _blobClientMock.Object;

            await Task.CompletedTask;
        }
    }
}
