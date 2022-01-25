using Moq;
using NUnit.Framework;
using System.Threading.Tasks;
using VideoApi.Services.Contracts;
using System;
using VideoApi.Contract.Requests;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using FluentAssertions;
using VideoApi.Services.Exceptions;

namespace VideoApi.UnitTests.Controllers.Conference
{
    public class ReconcileFilesInStorageTests : ConferenceControllerTestBase
    {

        [Test]
        public async Task Should_Check_Storage_For_Files_With_Prefix_And_Match_Count()
        {
            var guid1 = Guid.NewGuid();

            AzureStorageServiceFactoryMock.Setup(x => x.Create(AzureStorageServiceType.Vh)).Returns(AzureStorageServiceMock.Object);
            AudioPlatformServiceMock.Reset();
            AzureStorageServiceMock.Reset();

            AzureStorageServiceMock.Setup(x => x.ReconcileFilesInStorage(It.IsAny<string>(), It.IsAny<int>())).ReturnsAsync(true);

            AudioFilesInStorageRequest request = new AudioFilesInStorageRequest() { FileNamePrefix = guid1.ToString(), FilesCount = 1 };

            var requestResponse = await Controller.ReconcileAudioFilesInStorage(request);

            AzureStorageServiceMock.Verify(x => x.ReconcileFilesInStorage(It.IsAny<string>(), It.IsAny<int>()), Times.Once);

            var typedResult = (OkObjectResult)requestResponse;
            typedResult.StatusCode.Should().Be((int)HttpStatusCode.OK);

        }

        [Test]
        public void Should_Throw_Exception_With_Null_Request()
        {
            AzureStorageServiceFactoryMock.Setup(x => x.Create(AzureStorageServiceType.Vh)).Returns(AzureStorageServiceMock.Object);
            AudioPlatformServiceMock.Reset();
            AzureStorageServiceMock.Reset();

            AzureStorageServiceMock.Setup(x => x.ReconcileFilesInStorage(It.IsAny<string>(), It.IsAny<int>())).ReturnsAsync(true);

            AudioFilesInStorageRequest request = null;
            var msg = $"ReconcileFilesInStorage - File Name prefix is required.";

            Assert.That(async () => await Controller.ReconcileAudioFilesInStorage(request), Throws.TypeOf<AudioPlatformFileNotFoundException>().With.Message.EqualTo(msg));

            AzureStorageServiceMock.Verify(x => x.ReconcileFilesInStorage(It.IsAny<string>(), It.IsAny<int>()), Times.Never);

        }

        [Test]
        public void Should_Catch_Exception_When_Service_Throws_Exception()
        {
            var guid1 = Guid.NewGuid();

            AzureStorageServiceFactoryMock.Setup(x => x.Create(AzureStorageServiceType.Vh)).Returns(AzureStorageServiceMock.Object);

            AudioPlatformServiceMock.Reset();
            AzureStorageServiceMock.Reset();

            AzureStorageServiceMock.Setup(x => x.ReconcileFilesInStorage(It.IsAny<string>(), It.IsAny<int>())).Throws(new AudioPlatformFileNotFoundException(It.IsAny<string>(), HttpStatusCode.NotFound));

            AudioFilesInStorageRequest request = new AudioFilesInStorageRequest() { FileNamePrefix = guid1.ToString(), FilesCount = 1 };
                        
            Assert.That(async () => await Controller.ReconcileAudioFilesInStorage(request), Throws.TypeOf<AudioPlatformFileNotFoundException>());

            AzureStorageServiceMock.Verify(x => x.ReconcileFilesInStorage(It.IsAny<string>(), It.IsAny<int>()), Times.Once);

        }

    }
}
