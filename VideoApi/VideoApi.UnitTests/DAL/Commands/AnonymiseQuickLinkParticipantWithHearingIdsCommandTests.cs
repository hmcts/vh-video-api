using System;
using System.Collections.Generic;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using VideoApi.DAL;
using VideoApi.DAL.Commands;
using VideoApi.DAL.Exceptions;
using VideoApi.Domain;
using VideoApi.Domain.Enums;
using Task = System.Threading.Tasks.Task;

namespace VideoApi.UnitTests.DAL.Commands
{
    public class AnonymiseQuickLinkParticipantWithHearingIdsCommandTests : EfCoreSetup
    {
        private AnonymiseQuickLinkParticipantWithHearingIdsCommandHandler _commandHandler;
        private Conference _conference;
        private List<Conference> _conferences;
        private List<Participant> _participants;
        private Participant _quickLinkObserver, _quickLinkParticipant;

        [SetUp]
        public async Task SetUp()
        {
            _commandHandler = new AnonymiseQuickLinkParticipantWithHearingIdsCommandHandler(videoApiDbContext);

            _quickLinkObserver = DomainModelFactoryForTests.CreateParticipant();
            _quickLinkParticipant = DomainModelFactoryForTests.CreateParticipant();
            _conference = DomainModelFactoryForTests.CreateConference();

            _conference.AddParticipant(_quickLinkObserver);
            _conference.AddParticipant(_quickLinkParticipant);

            _participants = new List<Participant> { _quickLinkObserver, _quickLinkParticipant };

            await videoApiDbContext.Participants.AddRangeAsync(_quickLinkObserver, _quickLinkParticipant);

            _conferences = new List<Conference> { _conference };

            await videoApiDbContext.Conferences.AddAsync(_conference);

            await videoApiDbContext.SaveChangesAsync();
        }

        [TearDown]
        public async Task TearDown()
        {
            videoApiDbContext.Conferences.RemoveRange(_conferences);
            videoApiDbContext.Participants.RemoveRange(_participants);

            await videoApiDbContext.SaveChangesAsync();
        }

        [Test]
        public async Task Anonymises_Quick_Link_Participants_For_Specified_Hearing_Ids()
        {
            _quickLinkObserver.UserRole = UserRole.QuickLinkObserver;
            _quickLinkParticipant.UserRole = UserRole.QuickLinkParticipant;

            videoApiDbContext.Participants.UpdateRange(_quickLinkObserver, _quickLinkParticipant);
            await videoApiDbContext.SaveChangesAsync();

            var quickLinkObserverBeforeAnonymisation =
                DomainModelFactoryForTests.CreateParticipantCopyForAssertion(_quickLinkObserver);
            var quickLinkParticipantBeforeAnonymisation =
                DomainModelFactoryForTests.CreateParticipantCopyForAssertion(_quickLinkParticipant);

            await _commandHandler.Handle(new AnonymiseQuickLinkParticipantWithHearingIdsCommand
            {
                HearingIds = new List<Guid> { _conference.HearingRefId }
            });

            var processedQuickLinkObserver =
                await videoApiDbContext.Participants.SingleOrDefaultAsync(p => p.Id == _quickLinkObserver.Id);
            var processedQuickLinkParticipant =
                await videoApiDbContext.Participants.SingleOrDefaultAsync(p => p.Id == _quickLinkParticipant.Id);
            AssertParticipantFields(processedQuickLinkObserver, quickLinkObserverBeforeAnonymisation);
            AssertParticipantFields(processedQuickLinkParticipant, quickLinkParticipantBeforeAnonymisation);
        }

        [Test]
        public async Task Does_Not_Anonymise_Anonymised_Quick_Link_Participants()
        {
            _quickLinkObserver.UserRole = UserRole.QuickLinkObserver;
            _quickLinkParticipant.UserRole = UserRole.QuickLinkParticipant;
            _quickLinkParticipant.Username += Constants.AnonymisedUsernameSuffix;

            videoApiDbContext.Participants.Update(_quickLinkParticipant);
            await videoApiDbContext.SaveChangesAsync();

            var quickLinkObserverBeforeAnonymisation =
                DomainModelFactoryForTests.CreateParticipantCopyForAssertion(_quickLinkObserver);
            var quickLinkParticipantBeforeAnonymisation =
                DomainModelFactoryForTests.CreateParticipantCopyForAssertion(_quickLinkParticipant);

            await _commandHandler.Handle(new AnonymiseQuickLinkParticipantWithHearingIdsCommand
            {
                HearingIds = new List<Guid> { _conference.HearingRefId }
            });

            var quickLinkParticipantFromContext =
                await videoApiDbContext.Participants.SingleOrDefaultAsync(p => p.Id == _quickLinkParticipant.Id);
            var processedQuickLinkObserver =
                await videoApiDbContext.Participants.SingleOrDefaultAsync(p => p.Id == _quickLinkObserver.Id);

            quickLinkParticipantBeforeAnonymisation.DisplayName.Should()
                .Be(quickLinkParticipantFromContext.DisplayName);
            quickLinkParticipantBeforeAnonymisation.Username.Should().Be(quickLinkParticipantFromContext.Username);
            quickLinkParticipantBeforeAnonymisation.Name.Should().Be(quickLinkParticipantFromContext.Name);
            AssertParticipantFields(processedQuickLinkObserver, quickLinkObserverBeforeAnonymisation);
        }

        [Test]
        public void Throws_Participant_Not_Found_Exception()
        {
            Assert.ThrowsAsync<ConferenceNotFoundException>(() =>
                _commandHandler.Handle(new AnonymiseQuickLinkParticipantWithHearingIdsCommand
                    { HearingIds = new List<Guid> { Guid.NewGuid() } }));
        }

        private void AssertParticipantFields(Participant processedParticipant,
            Participant participantBeforeAnonymisation)
        {
            processedParticipant.DisplayName.Should().NotContain(participantBeforeAnonymisation.DisplayName);
            processedParticipant.Username.Should().NotContain(participantBeforeAnonymisation.Username);
            processedParticipant.Name.Should().NotContain(participantBeforeAnonymisation.Name);
            processedParticipant.Username.Should().Contain(Constants.AnonymisedUsernameSuffix);
        }
    }
}
