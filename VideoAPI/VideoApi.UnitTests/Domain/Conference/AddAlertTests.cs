using System;
using FluentAssertions;
using NUnit.Framework;
using Testing.Common.Helper.Builders.Domain;
using VideoApi.Domain.Enums;

namespace VideoApi.UnitTests.Domain.Conference
{
    public class AddAlertTests
    {
        [Test]
        public void should_add_alert_to_conference_as_todo()
        {
            var conference = new ConferenceBuilder().Build();
            var beforeCount = conference.GetTasks().Count;
            
            conference.AddTask(TaskType.Judge, "Auto");
            var afterCount = conference.GetTasks().Count;

            afterCount.Should().BeGreaterThan(beforeCount);
        }
    }
}