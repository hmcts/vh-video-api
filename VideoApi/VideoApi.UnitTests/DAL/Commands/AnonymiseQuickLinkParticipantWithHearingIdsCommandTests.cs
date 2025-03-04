using System;
using System.Collections.Generic;
using System.Linq;
using VideoApi.DAL.Commands;
using VideoApi.Domain;
using VideoApi.Domain.Enums;
using Task = System.Threading.Tasks.Task;

namespace VideoApi.UnitTests.DAL.Commands
{
    public class AnonymiseQuickLinkParticipantWithHearingIdsCommandTests : EfCoreSetup
    {
        private AnonymiseQuickLinkParticipantWithHearingIdsCommandHandler _commandHandler;
        private Conference _conference;
        private Guid _conferenceIdForTest;
        private List<Conference> _conferences;
        private ParticipantBase _quickLinkObserver, _quickLinkParticipant, _individualParticipant;

        [SetUp]
        public async Task SetUp()
        {
            _commandHandler = new AnonymiseQuickLinkParticipantWithHearingIdsCommandHandler(videoApiDbContext);

            _individualParticipant = DomainModelFactoryForTests.CreateParticipant();
            _quickLinkObserver = DomainModelFactoryForTests.CreateQuickLinkParticipant(UserRole.QuickLinkObserver);
            _quickLinkParticipant =
                DomainModelFactoryForTests.CreateQuickLinkParticipant(UserRole.QuickLinkParticipant);
            _conference = DomainModelFactoryForTests.CreateConference();
            _conferenceIdForTest = _conference.Id;

            _conference.AddParticipant(_individualParticipant);
            _conference.AddParticipant(_quickLinkObserver);
            _conference.AddParticipant(_quickLinkParticipant);

            _conferences = new List<Conference> { _conference };

            await videoApiDbContext.Conferences.AddAsync(_conference);

            await videoApiDbContext.SaveChangesAsync();
        }

        [TearDown]
        public async Task TearDown()
        {
            videoApiDbContext.Conferences.RemoveRange(_conferences);

            await videoApiDbContext.SaveChangesAsync();
        }

        [Test]
        public async Task Anonymises_Quick_Link_Participants_For_Specified_Hearing_Ids()
        {
            var individualParticipantBeforeAnonymisation =
                DomainModelFactoryForTests.CreateParticipantCopyForAssertion((Participant) _individualParticipant);
            var quickLinkObserverBeforeAnonymisation =
                DomainModelFactoryForTests.CreateQuickLinkParticipantCopyForAssertion(_quickLinkObserver);
            var quickLinkParticipantBeforeAnonymisation =
                DomainModelFactoryForTests.CreateQuickLinkParticipantCopyForAssertion(_quickLinkParticipant);

            await _commandHandler.Handle(new AnonymiseQuickLinkParticipantWithHearingIdsCommand
            {
                HearingIds = new List<Guid> { _conference.HearingRefId }
            });

            var getUpdatedParticipantsAfterAnonymisationProcess =
                videoApiDbContext.Conferences.First(c => c.Id == _conferenceIdForTest).GetParticipants();

            var processedIndividualParticipant =
                getUpdatedParticipantsAfterAnonymisationProcess.SingleOrDefault(p => p.Id == _individualParticipant.Id);
            var processedQuickLinkParticipant =
                getUpdatedParticipantsAfterAnonymisationProcess.SingleOrDefault(p => p.Id == _quickLinkParticipant.Id);
            var processedQuickLinkObserver =
                getUpdatedParticipantsAfterAnonymisationProcess.SingleOrDefault(p => p.Id == _quickLinkObserver.Id);

            individualParticipantBeforeAnonymisation.DisplayName.Should()
                .Be(processedIndividualParticipant.DisplayName);
            individualParticipantBeforeAnonymisation.Username.Should().Be(processedIndividualParticipant.Username);
            AssertParticipantFields(processedQuickLinkObserver, quickLinkObserverBeforeAnonymisation);
            AssertParticipantFields(processedQuickLinkParticipant, quickLinkParticipantBeforeAnonymisation);
        }

        [Test]
        public async Task Does_Not_Anonymise_Anonymised_Quick_Link_Participants()
        {
            var quickLinkParticipant =
                videoApiDbContext.Conferences.First(c => c.Id == _conferenceIdForTest).GetParticipants()
                    .FirstOrDefault(p => p.Id == _quickLinkParticipant.Id);

            quickLinkParticipant.Username += Constants.AnonymisedUsernameSuffix;

            videoApiDbContext.Conferences.Update(_conference);
            await videoApiDbContext.SaveChangesAsync();

            var quickLinkObserverBeforeAnonymisation =
                DomainModelFactoryForTests.CreateQuickLinkParticipantCopyForAssertion(_quickLinkObserver);
            var quickLinkParticipantBeforeAnonymisation =
                DomainModelFactoryForTests.CreateQuickLinkParticipantCopyForAssertion(_quickLinkParticipant);

            await _commandHandler.Handle(new AnonymiseQuickLinkParticipantWithHearingIdsCommand
            {
                HearingIds = new List<Guid> { _conference.HearingRefId }
            });

            var quickLinkParticipantFromContext = videoApiDbContext.Conferences.First().GetParticipants()
                .SingleOrDefault(p => p.Id == _quickLinkParticipant.Id);
            var processedQuickLinkObserver = videoApiDbContext.Conferences.First().GetParticipants()
                .SingleOrDefault(p => p.Id == _quickLinkObserver.Id);

            quickLinkParticipantBeforeAnonymisation.DisplayName.Should()
                .Be(quickLinkParticipantFromContext.DisplayName);
            quickLinkParticipantBeforeAnonymisation.Username.Should().Be(quickLinkParticipantFromContext.Username);
            AssertParticipantFields(processedQuickLinkObserver, quickLinkObserverBeforeAnonymisation);
        }

        [Test]
        public async Task Does_Not_Throw_Exception_When_No_Conferences_Are_Found()
        {
            var quickLinkParticipantBeforeAnonymisation =
                DomainModelFactoryForTests.CreateQuickLinkParticipantCopyForAssertion(_quickLinkParticipant);

            await _commandHandler.Handle(new AnonymiseQuickLinkParticipantWithHearingIdsCommand
            {
                HearingIds = new List<Guid>()
            });

            var quickLinkParticipantFromContext = videoApiDbContext.Conferences.First().GetParticipants()
                .SingleOrDefault(p => p.Id == _quickLinkParticipant.Id);

            quickLinkParticipantBeforeAnonymisation.DisplayName.Should()
                .Be(quickLinkParticipantFromContext.DisplayName);
            quickLinkParticipantBeforeAnonymisation.Username.Should().Be(quickLinkParticipantFromContext.Username);
        }

        private static void AssertParticipantFields(ParticipantBase processedParticipant,
            ParticipantBase participantBeforeAnonymisation)
        {
            processedParticipant.DisplayName.Should().NotContain(participantBeforeAnonymisation.DisplayName);
            processedParticipant.Username.Should().NotContain(participantBeforeAnonymisation.Username);
            processedParticipant.Username.Should().Contain(Constants.AnonymisedUsernameSuffix);
        }
    }
}
