using NUnit.Framework;
using AcceptanceTests.Common.Api;
using Microsoft.Extensions.Configuration;
using VideoApi.Common.Configuration;

namespace VideoApi.IntegrationTests
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
        public void StartZap()
        {
            Zap.Start();
        }

        [OneTimeTearDown]
        public void ZapReport()
        {
            Zap.ReportAndShutDown("VideoApi-Integration", ServicesConfiguration.VideoApiUrl);
        }
    }
}
