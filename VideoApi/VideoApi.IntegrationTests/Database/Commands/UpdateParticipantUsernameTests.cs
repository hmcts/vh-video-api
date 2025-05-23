using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using VideoApi.DAL;
using VideoApi.DAL.Commands;
using VideoApi.DAL.Exceptions;

namespace VideoApi.IntegrationTests.Database.Commands
{
    public class UpdateParticipantUsernameTests : DatabaseTestsBase
    {
        private VideoApiDbContext _context;
        private UpdateParticipantUsernameCommandHandler _handler;
        private Guid _newConferenceId;
        
        [SetUp]
        public void Setup()
        {
            _context = new VideoApiDbContext(VideoBookingsDbContextOptions);
            _handler = new UpdateParticipantUsernameCommandHandler(_context);
            _newConferenceId = Guid.Empty;
        }
        
        [Test]
        public async Task Should_throw_participant_not_found_exception_when_participant_does_not_exist()
        {
            var seededConference = await TestDataManager.SeedConference();
            TestContext.WriteLine($"New seeded conference id: {seededConference.Id}");
            _newConferenceId = seededConference.Id;
            var participantId = Guid.NewGuid();
            var newUsername = "NewUsername";
            var command = new UpdateParticipantUsernameCommand(participantId, newUsername);
            Assert.ThrowsAsync<ParticipantNotFoundException>(() => _handler.Handle(command));
        }
        
        [Test]
        public async Task Should_update_participant_username_and_any_linked_endpoints()
        {
            var seededConference = await TestDataManager.SeedConferenceWithEndpoint();
            TestContext.WriteLine($"New seeded conference id: {seededConference.Id}");
            _newConferenceId = seededConference.Id;
            var participant = seededConference.GetParticipants()[0];
            const string newUsername = "NewUsername";
            
            var oldUsername = participant.Username;

            var command = new UpdateParticipantUsernameCommand(participant.Id, newUsername);
            await _handler.Handle(command);
            
            var conference = await _context.Conferences
                .Include(x => x.Participants)
                .Include(x => x.Endpoints)
                .Where(x => x.Participants.Any(p => p.Id == command.ParticipantId))
                .SingleAsync();

            var updatedParticipant = conference.GetParticipants().Single(x => x.Id == command.ParticipantId);
            
            updatedParticipant.Username.Should().NotBe(oldUsername);
            updatedParticipant.Username.Should().Be(newUsername);
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
