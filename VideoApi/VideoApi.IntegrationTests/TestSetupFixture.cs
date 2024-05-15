using NUnit.Framework;
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
                                                            .GetSection("Services")
                                                            .Get<ServicesConfiguration>();
    }
}
