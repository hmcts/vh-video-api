using System;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using Testing.Common.Helper.Builders.Domain;
using VideoApi.DAL;
using VideoApi.DAL.Commands;
using VideoApi.DAL.Exceptions;
using VideoApi.DAL.Queries;
using VideoApi.Domain.Enums;

namespace VideoApi.IntegrationTests.Database.Commands
{
    public class UpdateVirtualCourtCommandTests : DatabaseTestsBase
    {
        private UpdateVirtualCourtCommandHandler _handler;
        private GetConferenceByIdQueryHandler _conferenceByIdHandler;
        private Guid _newConferenceId;
        
        [SetUp]
        public void Setup()
        {
            var context = new VideoApiDbContext(VideoBookingsDbContextOptions);
            _handler = new UpdateVirtualCourtCommandHandler(context);
            _conferenceByIdHandler = new GetConferenceByIdQueryHandler(context);
            _newConferenceId = Guid.Empty;
        }
        
        [Test]
        public void should_throw_conference_not_found_exception_when_conference_does_not_exist()
        {
            var conferenceId = Guid.NewGuid();
            var command = BuildCommand(conferenceId);
            
            Assert.ThrowsAsync<ConferenceNotFoundException>(() => _handler.Handle(command));
        }
        
        [Test]
        public async Task should_add_conference_virtual_court()
        {
            var seededConference = await TestDataManager.SeedConference();
            TestContext.WriteLine($"New seeded conference id: {seededConference.Id}");
            _newConferenceId = seededConference.Id;
            var command = BuildCommand(_newConferenceId);
            await _handler.Handle(command);
            
            var updatedConference = await _conferenceByIdHandler.Handle(new GetConferenceByIdQuery(_newConferenceId));
            updatedConference.VirtualCourt.Should().NotBeNull();
            updatedConference.VirtualCourt.AdminUri.Should().Be(command.AdminUri);
            updatedConference.VirtualCourt.JudgeUri.Should().Be(command.JudgeUri);
            updatedConference.VirtualCourt.ParticipantUri.Should().Be(command.ParticipantUri);
            updatedConference.VirtualCourt.PexipNode.Should().Be(command.PexipNode);
        }
        
        [Test]
        public async Task should_update_conference_virtual_court()
        {
            var conferenceWithVirtualCourt = new ConferenceBuilder(true)
                .WithParticipant(UserRole.Individual, "Claimant")
                .WithVirtualCourt().Build();
            var seededConference = await TestDataManager.SeedConference(conferenceWithVirtualCourt);
            TestContext.WriteLine($"New seeded conference id: {seededConference.Id}");
            _newConferenceId = seededConference.Id;
            var command = BuildCommand(_newConferenceId);
            await _handler.Handle(command);
            
            var updatedConference = await _conferenceByIdHandler.Handle(new GetConferenceByIdQuery(_newConferenceId));
            updatedConference.VirtualCourt.Should().NotBeNull();
            updatedConference.VirtualCourt.AdminUri.Should().Be(command.AdminUri);
            updatedConference.VirtualCourt.JudgeUri.Should().Be(command.JudgeUri);
            updatedConference.VirtualCourt.ParticipantUri.Should().Be(command.ParticipantUri);
            updatedConference.VirtualCourt.PexipNode.Should().Be(command.PexipNode);
        }

        private UpdateVirtualCourtCommand BuildCommand(Guid conferenceId)
        {
            var adminUri = "https://testjoin.poc.hearings.hmcts.net/viju/#/?conference=ola@hearings.hmcts.net&output=embed";
            var judgeUri = "https://testjoin.poc.hearings.hmcts.net/viju/#/?conference=ola@hearings.hmcts.net&output=embed";
            var participantUri =
                "https://testjoin.poc.hearings.hmcts.net/viju/#/?conference=ola@hearings.hmcts.net&output=embed";
            var pexipNode = "testjoin.poc.hearings.hmcts.net";
            return new UpdateVirtualCourtCommand(conferenceId, adminUri, judgeUri, participantUri, pexipNode);
        }
        
        [TearDown]
        public async Task TearDown()
        {
            if (_newConferenceId != Guid.Empty)
            {
                TestContext.WriteLine($"Removing test conference {_newConferenceId}");
                await TestDataManager.RemoveConference(_newConferenceId);
            }
        }
    }
}