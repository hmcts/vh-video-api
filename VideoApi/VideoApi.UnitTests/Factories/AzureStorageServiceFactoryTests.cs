using System;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using VideoApi.Factories;
using VideoApi.Services.Contracts;

namespace VideoApi.UnitTests.Factories
{
    public class AzureStorageServiceFactoryTests
    {
        private readonly AzureStorageServiceFactory _factory;
        
        public AzureStorageServiceFactoryTests()
        {
            var vhServiceMock = new Mock<IAzureStorageService>();
            vhServiceMock.Setup(x => x.AzureStorageServiceType).Returns(AzureStorageServiceType.Vh);
            
            var cvpServiceMock = new Mock<IAzureStorageService>();
            cvpServiceMock.Setup(x => x.AzureStorageServiceType).Returns(AzureStorageServiceType.Cvp);
            
            _factory = new AzureStorageServiceFactory(new []{ vhServiceMock.Object, cvpServiceMock.Object });
        }

        [Test]
        public void Create_gets_vh_service()
        {
            var service = _factory.Create(AzureStorageServiceType.Vh);

            service.Should().NotBeNull();
            service.AzureStorageServiceType.Should().Be(AzureStorageServiceType.Vh);
        }

        [Test]
        public void Create_gets_cvp_service()
        {
            var service = _factory.Create(AzureStorageServiceType.Cvp);

            service.Should().NotBeNull();
            service.AzureStorageServiceType.Should().Be(AzureStorageServiceType.Cvp);
        }

        [Test]
        public void Create_throws_exception_for_unknown_service_type()
        {
            Assert.Throws<NotImplementedException>(() => _factory.Create((AzureStorageServiceType) 999999));
        }
    }
}
