using System;
using Faker;
using FluentAssertions;
using NUnit.Framework;
using Testing.Common.Helper.Builders.Domain;

namespace VideoApi.UnitTests.Domain.Conference
{
    public class AddMessageTests
    {
        [Test]
        public void should_add_new_message_to_hearing()
        {
            var conference = new ConferenceBuilder().Build();
            var beforeCount = conference.GetMessages().Count;
            conference.AddMessage(Internet.Email(), Internet.Email(), Internet.DomainWord());

            var afterCount = conference.GetMessages().Count;
            afterCount.Should().BeGreaterThan(beforeCount);

            //Add another message
            beforeCount = afterCount;
            conference.AddMessage(Internet.Email(), Internet.Email(), Internet.DomainWord());
            var newCount = conference.GetMessages().Count;
            newCount.Should().Be(beforeCount + 1);
        }
    }
}
