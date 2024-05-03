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
                                                            .GetSection("Services")
                                                            .Get<ServicesConfiguration>();
    }
}
