using System;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using Testing.Common.Helper.Builders.Domain;
using VideoApi.Domain;
using VideoApi.Domain.Validations;

namespace VideoApi.UnitTests.Domain.Conference
{
    public class RemoveEndpointTests
    {
        [Test]
        public void should_remove_existing_endpoint()
        {
            var displayName = "remove test";
            var sipAddress = "2334324@sip.com";
            var conference = new ConferenceBuilder().WithEndpoint(displayName, sipAddress).Build();
            
            var beforeCount = conference.GetEndpoints().Count;

            var endpoint = conference.GetEndpoints().First();

            conference.RemoveEndpoint(endpoint);

            var afterCount = conference.GetEndpoints().Count;
            afterCount.Should().BeLessThan(beforeCount);
        }

        [Test]
        public void should_throw_exception_when_removing_an_endpoint_that_does_not_exist()
        {
            var displayName = "remove test";
            var sipAddress = "2334324@sip.com";
            var conference = new ConferenceBuilder().WithEndpoint(displayName, sipAddress).Build();

            var beforeCount = conference.GetEndpoints().Count;
            var endpoint = new Endpoint("Display", "test@sip.com", "1234", "Defence Sol");
            Action action = () =>  conference.RemoveEndpoint(endpoint);
            
            action.Should().Throw<DomainRuleException>().Where(x =>
                x.ValidationFailures.Any(v => v.Message == "Endpoint does not exist in conference"));
            
            var afterCount = conference.GetEndpoints().Count;
            beforeCount.Should().Be(afterCount);
        }
    }
}
