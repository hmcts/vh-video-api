using System;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
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
            var linkedId = _participantA.LinkedParticipants.Select(x => x.LinkedId);
            linkedId.Should().BeEquivalentTo(_participantB.Id);
        }
        
        [Test]
        public void Should_throw_exception_when_link_already_exists()
        {
            _participantA.AddLink(_participantB.Id, LinkedParticipantType.Interpreter);

            _participantA.Invoking(x => x.AddLink(_participantB.Id, LinkedParticipantType.Interpreter)).Should()
                .Throw<DomainRuleException>();
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
