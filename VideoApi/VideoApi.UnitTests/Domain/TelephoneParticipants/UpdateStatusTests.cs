using System;
using Testing.Common.Helper.Builders.Domain;
using VideoApi.Domain;
using VideoApi.Domain.Enums;

namespace VideoApi.UnitTests.Domain.TelephoneParticipants;

public class UpdateStatusTests
{
    [Test]
    public void should_update_status()
    {
        var conference = new ConferenceBuilder().Build();
        var telephoneParticipant = new TelephoneParticipant(Guid.NewGuid(), "Anonymous", conference);
        var status = TelephoneState.Disconnected;
        telephoneParticipant.UpdateStatus(status);
        telephoneParticipant.State.Should().Be(status);
    }

}
