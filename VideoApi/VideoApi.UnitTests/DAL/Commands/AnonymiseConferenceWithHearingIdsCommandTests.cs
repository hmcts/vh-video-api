using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using VideoApi.DAL;
using VideoApi.DAL.Commands;
using VideoApi.DAL.Exceptions;
using VideoApi.Domain;
using Task = System.Threading.Tasks.Task;

namespace VideoApi.UnitTests.DAL.Commands
{
    public class AnonymiseConferenceWithHearingIdsCommandTests : EfCoreSetup
    {
        private AnonymiseConferenceWithHearingIdsCommandHandler _commandHandler;
        private Conference _conference1, _conference2;
        private List<Conference> _conferences;
        private List<Participant> _participants;

        [SetUp]
        public async Task SetUp()
        {
            _commandHandler = new AnonymiseConferenceWithHearingIdsCommandHandler(videoApiDbContext);

            _conference1 = DomainModelFactoryForTests.CreateConference();
            _conference2 = DomainModelFactoryForTests.CreateConference();
            _conferences = new List<Conference> { _conference1, _conference2 };
            _participants = new List<Participant>();

            await videoApiDbContext.Participants.AddRangeAsync(_participants);
            await videoApiDbContext.Conferences.AddRangeAsync(_conference1, _conference2);

            await videoApiDbContext.SaveChangesAsync();
        }

        [TearDown]
        public void TearDown()
        {
            videoApiDbContext.Participants.RemoveRange(_participants);
            videoApiDbContext.Conferences.RemoveRange(_conferences);
        }

        [Test]
        public async Task Anonymises_CaseName_With_Specified_Hearing_Ids()
        {
            var caseNameBeforeAnonymisationForConference1 = _conference1.CaseName;
            var caseNameBeforeAnonymisationForConference2 = _conference2.CaseName;

            await _commandHandler.Handle(new AnonymiseConferenceWithHearingIdsCommand
            {
                HearingIds = new List<Guid>
                {
                    _conference1.HearingRefId,
                    _conference2.HearingRefId
                }
            });

            var processedConferences = videoApiDbContext.Conferences
                .Where(c => c.Id == _conference1.Id || c.Id == _conference2.Id).ToList();

            foreach (var conference in processedConferences)
                conference.CaseName.Should().NotContain(caseNameBeforeAnonymisationForConference1).And
                    .NotContain(caseNameBeforeAnonymisationForConference2);
        }

        [Test]
        public async Task Does_Not_Anonymise_When_Conference_Associated_With_An_Anonymised_Participant()
        {
            var participant = new Participant { Username = $"someUser{Constants.AnonymisedUsernameSuffix}" };
            _participants.Add(participant);
            videoApiDbContext.Participants.Add(participant);
            _conference1.AddParticipant(participant);

            videoApiDbContext.Update(_conference1);

            await videoApiDbContext.SaveChangesAsync();

            var caseNameBeforeAnonymisationForConference1 = _conference1.CaseName;

            await _commandHandler.Handle(new AnonymiseConferenceWithHearingIdsCommand
            {
                HearingIds = new List<Guid>
                {
                    _conference1.HearingRefId
                }
            });

            var processedConference =
                await videoApiDbContext.Conferences.FirstOrDefaultAsync(c => c.Id == _conference1.Id);

            processedConference.CaseName.Should().Be(caseNameBeforeAnonymisationForConference1);
        }
        
        [Test]
        public void Throws_Participant_Not_Found_Exception()
        {
            Assert.ThrowsAsync<ConferenceNotFoundException>(() =>
                _commandHandler.Handle(new AnonymiseConferenceWithHearingIdsCommand
                    { HearingIds = new List<Guid> { Guid.NewGuid() } }));
        }
    }
}
