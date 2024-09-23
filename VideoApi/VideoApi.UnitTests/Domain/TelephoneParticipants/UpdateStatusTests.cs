using VideoApi.Domain;
using VideoApi.Domain.Enums;

namespace VideoApi.UnitTests.Domain.TelephoneParticipants;

public class UpdateStatusTests
{
    [Test]
    public void should_update_status()
    {
        var telephoneParticipant = new TelephoneParticipant("Anonymous");
        var status = TelephoneState.Disconnected;
        telephoneParticipant.UpdateStatus(status);
        telephoneParticipant.State.Should().Be(status);
    }

}
