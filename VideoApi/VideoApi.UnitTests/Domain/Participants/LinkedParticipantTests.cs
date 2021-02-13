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
        private Participant _individual;
        private Participant _linkedIndividual;

        [SetUp]
        public void SetUp()
        {
            _individual = new ParticipantBuilder().Build();
            _linkedIndividual = new ParticipantBuilder().Build();
        }
        
        [Test]
        public void Should_Add_Link()
        {
            _individual.AddLink(_linkedIndividual.Id, LinkedParticipantType.Interpreter);
            var linkedId = _individual.LinkedParticipants.Select(x => x.LinkedId);

            linkedId.Should().BeEquivalentTo(_linkedIndividual.Id);
        }
        
        [Test]
        public void Should_Throw_Exception_When_Link_Already_Exists()
        {
            _individual.AddLink(_linkedIndividual.Id, LinkedParticipantType.Interpreter);

            _individual.Invoking(x => x.AddLink(_linkedIndividual.Id, LinkedParticipantType.Interpreter)).Should()
                .Throw<DomainRuleException>();
        }
    }
}
