using System;
using System.Linq;
using Testing.Common.Helper.Builders.Domain;
using VideoApi.Domain;
using VideoApi.Domain.Validations;

namespace VideoApi.UnitTests.Domain.Conference;

public class RemoveTelephoneParticipantTests
{
    [Test]
    public void should_remove_a_telephone_participant()
    {
        var conference = new ConferenceBuilder().Build();
        var telephoneParticipant = new TelephoneParticipant(Guid.NewGuid(), "Anonymous");
        
        conference.AddTelephoneParticipant(telephoneParticipant);
        
        var beforeCount = conference.GetTelephoneParticipants().Count;
        
        conference.RemoveTelephoneParticipant(telephoneParticipant);
        
        var afterCount = conference.GetTelephoneParticipants().Count;
        afterCount.Should().BeLessThan(beforeCount);
    }
    
    [Test]
    public void should_not_remove_a_telephone_participant_that_does_not_exist()
    {
        var conference = new ConferenceBuilder().Build();
        var telephoneParticipant = new TelephoneParticipant(Guid.NewGuid(), "Anonymous");
        
        var action = () => conference.RemoveTelephoneParticipant(telephoneParticipant);
        
        action.Should().Throw<DomainRuleException>().Where(x=> 
            x.ValidationFailures.Any(v => v.Message == "Telephone participant does not exist in conference"));
    }
}
