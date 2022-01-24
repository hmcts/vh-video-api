using Moq;
using NUnit.Framework;
using System.Collections.Generic;
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
    public class CheckFilesExistsInStorageTests : ConferenceControllerTestBase
    {

        [Test]
        public async Task Should_Check_Storage_For_Files_With_Prefix_And_Match_Count()
        {
            var guid1 = Guid.NewGuid();

            var results = new List<string>() { guid1.ToString() };

            AzureStorageServiceFactoryMock.Setup(x => x.Create(AzureStorageServiceType.Vh)).Returns(AzureStorageServiceMock.Object);
            AudioPlatformServiceMock.Reset();
            AzureStorageServiceMock.Reset();

            AzureStorageServiceMock.Setup(x => x.GetAllBlobNamesByFilePathPrefix(It.IsAny<string>())).ReturnsAsync(results);
            AzureStorageServiceMock.Setup(x => x.GetAllEmptyBlobsByFilePathPrefix(It.IsAny<string>())).ReturnsAsync(new List<string>());

            AudioFilesInStorageRequest request = new AudioFilesInStorageRequest() { FileNamePrefix = guid1.ToString(), FilesCount = 1 };

            var requestResponse = await Controller.AudioFileExists(request);

            AzureStorageServiceMock.Verify(x => x.GetAllBlobNamesByFilePathPrefix(It.IsAny<string>()), Times.Once);
            AzureStorageServiceMock.Verify(x => x.GetAllEmptyBlobsByFilePathPrefix(It.IsAny<string>()), Times.Once);

            var typedResult = (OkObjectResult)requestResponse;
            typedResult.StatusCode.Should().Be((int)HttpStatusCode.OK);

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

            var requestResponse = await Controller.AudioFileExists(request);

            AzureStorageServiceMock.Verify(x => x.GetAllBlobNamesByFilePathPrefix(It.IsAny<string>()), Times.Once);
            AzureStorageServiceMock.Verify(x => x.GetAllEmptyBlobsByFilePathPrefix(It.IsAny<string>()), Times.Once);

            var typedResult = (OkObjectResult)requestResponse;
            typedResult.StatusCode.Should().Be((int)HttpStatusCode.OK);

        }

        [Test]
        public async Task Should_Throw_AudioPlatformFileNotFoundException_If_Storage_Returns_Less_Files_With_Parameter_FileCount()
        {
            var guid1 = Guid.NewGuid();

            var msg = $"CheckAudioFilesInStorage - File name prefix :" + guid1.ToString() + "  Expected: " + "1" + " Actual:" + "0";


            AzureStorageServiceFactoryMock.Setup(x => x.Create(AzureStorageServiceType.Vh)).Returns(AzureStorageServiceMock.Object);
            AudioPlatformServiceMock.Reset();
            AzureStorageServiceMock.Reset();

            AzureStorageServiceMock.Setup(x => x.GetAllBlobNamesByFilePathPrefix(It.IsAny<string>())).ReturnsAsync(new List<string>());
            AzureStorageServiceMock.Setup(x => x.GetAllEmptyBlobsByFilePathPrefix(It.IsAny<string>())).ReturnsAsync(new List<string>());

            AudioFilesInStorageRequest request = new AudioFilesInStorageRequest() { FileNamePrefix = guid1.ToString(), FilesCount = 1 };
                        
            Assert.That(async () => await Controller.AudioFileExists(request), Throws.TypeOf<AudioPlatformFileNotFoundException>().With.Message.EqualTo(msg));

            AzureStorageServiceMock.Verify(x => x.GetAllBlobNamesByFilePathPrefix(It.IsAny<string>()), Times.Once);
            AzureStorageServiceMock.Verify(x => x.GetAllEmptyBlobsByFilePathPrefix(It.IsAny<string>()), Times.Once);

            await Task.Delay(10);
        }

        [Test]
        public async Task Should_Throw_AudioPlatformFileNotFoundException_If_Storage_Returns_Any_Empty_Files()
        {
            var guid1 = Guid.NewGuid();

            var msg = $"CheckAudioFilesInStorage - File name prefix :" + guid1.ToString() + "  Expected: " + "1" + " Actual:" + "0";

            var results = new List<string>() { guid1.ToString()};

            AzureStorageServiceFactoryMock.Setup(x => x.Create(AzureStorageServiceType.Vh)).Returns(AzureStorageServiceMock.Object);
            AudioPlatformServiceMock.Reset();
            AzureStorageServiceMock.Reset();

            AzureStorageServiceMock.Setup(x => x.GetAllBlobNamesByFilePathPrefix(It.IsAny<string>())).ReturnsAsync(new List<string>());
            AzureStorageServiceMock.Setup(x => x.GetAllEmptyBlobsByFilePathPrefix(It.IsAny<string>())).ReturnsAsync(results);

            AudioFilesInStorageRequest request = new AudioFilesInStorageRequest() { FileNamePrefix = guid1.ToString(), FilesCount = 1 };

            Assert.That(async () => await Controller.AudioFileExists(request), Throws.TypeOf<AudioPlatformFileNotFoundException>().With.Message.EqualTo(msg));

            AzureStorageServiceMock.Verify(x => x.GetAllBlobNamesByFilePathPrefix(It.IsAny<string>()), Times.Once);
            AzureStorageServiceMock.Verify(x => x.GetAllEmptyBlobsByFilePathPrefix(It.IsAny<string>()), Times.Once);

            await Task.Delay(10);
        }
    }
}
