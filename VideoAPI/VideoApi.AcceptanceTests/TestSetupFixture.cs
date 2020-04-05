using AcceptanceTests.Common.Api;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using VideoApi.Common.Configuration;

namespace VideoApi.AcceptanceTests
{
    [SetUpFixture]
    public class TestSetupFixture
    {
        private ServicesConfiguration ServicesConfiguration => new ConfigurationBuilder()
                                                            .AddJsonFile("appsettings.json")
                                                            .Build()
                                                            .GetSection("VhServices")
                                                            .Get<ServicesConfiguration>();

        [OneTimeSetUp]
        public void ZapStart()
        {
            Zap.Start();
        }

        [OneTimeTearDown]
        public void ZapReport()
        {
            Zap.ReportAndShutDown("VideoApi - Acceptance",ServicesConfiguration.VideoApiUrl); 
        }
    }
}
