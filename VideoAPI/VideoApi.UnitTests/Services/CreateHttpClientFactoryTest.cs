using FluentAssertions;
using NUnit.Framework;
using System;
using VideoApi.Common.Configuration;
using VideoApi.Services.Clients;
using VideoApi.Services.Contracts;

namespace VideoApi.UnitTests.Services
{
    public class CreateHttpClientFactoryTest
    {
        private ICreateHttpClientFactory factory;

        [Test]
        public void Should_return_two_clients_model()
        {
            var _configuration = new WowzaConfiguration
            {
                RestApiEndpoint = "http://one.endpoint",
                RestApiEndpoint1 = "http://two.endpoint",
                HostName = "host",
                ServerName = "server"
            };

            factory = new CreateHttpClientFactory(_configuration);

            var result = factory.GetHttpClients();
            result.Should().NotBeNull();
            result.Count.Should().Be(2);
            result[0].HostName.Should().Be("host");
            result[0].ServerName.Should().Be("server");
            result[0].HttpClientForNode.Should().NotBeNull();
            result[1].HttpClientForNode.Should().NotBeNull();
            result[0].HttpClientForNode.BaseAddress.Should().Be(new Uri(_configuration.RestApiEndpoint));
            result[1].HttpClientForNode.BaseAddress.Should().Be(new Uri(_configuration.RestApiEndpoint1));

        }
    }
}
