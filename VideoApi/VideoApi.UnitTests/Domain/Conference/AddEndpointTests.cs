using System;
using System.Linq;
using Testing.Common.Helper.Builders.Domain;
using VideoApi.Domain;
using VideoApi.Domain.Enums;
using VideoApi.Domain.Validations;

namespace VideoApi.UnitTests.Domain.Conference
{
    public class AddEndpointTests
    {
        [Test]
        public void should_add_endpoint()
        {
            var conference = new ConferenceBuilder().Build();
            var beforeCount = conference.GetEndpoints().Count;
            var endpoint = new Endpoint("Display", "test@sip.com", "1234", "Defence Sol");
            conference.AddEndpoint(endpoint);
            var afterCount = conference.GetEndpoints().Count;
            afterCount.Should().BeGreaterThan(beforeCount);

            endpoint.State.Should().Be(EndpointState.NotYetJoined);
            endpoint.DisplayName.Should().Be("Display");
            endpoint.SipAddress.Should().Be("test@sip.com");
            endpoint.Pin.Should().Be("1234");
            endpoint.DefenceAdvocate.Should().Be("Defence Sol");
        }

        [Test]
        public void should_not_add_same_endpoint_twice()
        {
            var conference = new ConferenceBuilder().Build();
            var endpoint = new Endpoint("Display", "test@sip.com", "1234", "Defence Sol");
            conference.AddEndpoint(endpoint);
            
            Action action = () => conference.AddEndpoint(endpoint);

            action.Should().Throw<DomainRuleException>().Where(x =>
                x.ValidationFailures.Any(v => v.Message == "Endpoint already exists in conference"));

        }
    }
}
