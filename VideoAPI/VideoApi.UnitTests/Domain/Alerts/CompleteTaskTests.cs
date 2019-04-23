using System;
using FluentAssertions;
using NUnit.Framework;
using VideoApi.Domain;
using VideoApi.Domain.Enums;

namespace VideoApi.UnitTests.Domain.Alerts
{
    public class CompleteTaskTests
    {
        [Test]
        public void should_not_be_completed_by_default()
        {
            var alert = new Alert("Something happened", AlertType.Participant);
            alert.Status.Should().Be(AlertStatus.ToDo);
            alert.Updated.Should().BeNull();
        }
        
        [Test]
        public void should_update_status_to_done()
        {
            var alert = new Alert("Something happened", AlertType.Participant);
            const string user = "Test User";
            
            alert.CompleteTask(user);
            alert.Status.Should().Be(AlertStatus.Done);
            alert.UpdatedBy.Should().Be(user);
            alert.Updated.Should().NotBeNull();
        }
    }
}