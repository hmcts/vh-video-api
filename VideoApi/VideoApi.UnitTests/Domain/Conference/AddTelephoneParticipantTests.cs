using System;
using System.Linq;
using Testing.Common.Helper.Builders.Domain;
using VideoApi.Domain;
using VideoApi.Domain.Validations;

namespace VideoApi.UnitTests.Domain.Conference;

public class AddTelephoneParticipantTests
{
    [Test]
    public void should_add_a_telephone_participant()
    {
        var conference = new ConferenceBuilder().Build();
        var beforeCount = conference.GetTelephoneParticipants().Count;
        var telephoneParticipant = new TelephoneParticipant(Guid.NewGuid(), "Anonymous", conference);
        
        conference.AddTelephoneParticipant(telephoneParticipant);
        
        var afterCount = conference.GetTelephoneParticipants().Count;
        afterCount.Should().BeGreaterThan(beforeCount);
    }
    
    [Test]
    public void should_not_add_same_telephone_participant_twice()
    {
        var conference = new ConferenceBuilder().Build();
        var telephoneParticipant = new TelephoneParticipant(Guid.NewGuid(), "Anonymous", conference);
        
        conference.AddTelephoneParticipant(telephoneParticipant);
        
        Action action = () => conference.AddTelephoneParticipant(telephoneParticipant);
        
        action.Should().Throw<DomainRuleException>().Where(x=> 
            x.ValidationFailures.Any(v => v.Message == "Telephone participant already exists in conference"));
    }
}
