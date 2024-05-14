using System;
using System.Linq;
using Testing.Common.Helper.Builders.Domain;
using VideoApi.Domain;
using VideoApi.Domain.Enums;
using VideoApi.Domain.Validations;

namespace VideoApi.UnitTests.Domain.Participants
{
    public class LinkedParticipantTests
    {
        private Participant _participantA;
        private Participant _participantB;

        [SetUp]
        public void SetUp()
        {
            _participantA = new ParticipantBuilder().Build();
            _participantB = new ParticipantBuilder().Build();
        }
        
        [Test]
        public void Should_add_link()
        {
            _participantA.AddLink(_participantB.Id, LinkedParticipantType.Interpreter);

            _participantA.LinkedParticipants.Should().HaveCount(1);
            _participantA.LinkedParticipants.Where(x => x.ParticipantId == _participantA.Id && x.LinkedId == _participantB.Id && x.Type == LinkedParticipantType.Interpreter).Should().HaveCount(1);
        }
        
        [Test]
        public void Should_not_add_when_link_already_exists()
        {
            _participantA.AddLink(_participantB.Id, LinkedParticipantType.Interpreter);
            var before = _participantA.LinkedParticipants.ToList();

            _participantA.Invoking(x => x.AddLink(_participantB.Id, LinkedParticipantType.Interpreter));

            var after = _participantA.LinkedParticipants.ToList();

            before.Should().BeEquivalentTo(after);
        }
        
        [Test]
        public void Should_remove_link()
        {
            _participantA.AddLink(_participantB.Id, LinkedParticipantType.Interpreter);
            _participantA.LinkedParticipants.Any().Should().BeTrue();
            
            _participantA.RemoveLink(_participantA.LinkedParticipants.First());
            _participantA.LinkedParticipants.Any().Should().BeFalse();
        }
        
        [Test]
        public void Should_throw_exception_when_link_to_remove_doesnt_exist()
        {
            _participantA.Invoking(
                x => x.RemoveLink(
                    new LinkedParticipant(
                        Guid.NewGuid(), 
                        Guid.NewGuid(), 
                        LinkedParticipantType.Interpreter)
                )
            ).Should().Throw<DomainRuleException>();
        }
    }
}
