using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using VideoApi.DAL;
using VideoApi.DAL.Commands;
using VideoApi.Domain;
using VideoApi.Domain.Enums;

namespace VideoApi.IntegrationTests.Database.Commands
{
    public class SaveEventCommandTests : DatabaseTestsBase
    {
        private SaveEventCommandHandler _handler;
        private Guid _newConferenceId;

        [SetUp]
        public void Setup()
        {
            var context = new VideoApiDbContext(VideoBookingsDbContextOptions);
            _handler = new SaveEventCommandHandler(context);
            _newConferenceId = Guid.Empty;
        }

        [TearDown]
        public async Task TearDown()
        {
            if (_newConferenceId != Guid.Empty)
            {
                TestContext.WriteLine($"Removing test conference {_newConferenceId}");
                await TestDataManager.RemoveConference(_newConferenceId);
            }

            await TestDataManager.RemoveEvents();
        }
        
        [Test]
        public async Task should_save_event()
        {
            var seededConference = await TestDataManager.SeedConference();
            TestContext.WriteLine($"New seeded conference id: {seededConference.Id}");
            _newConferenceId = seededConference.Id;
            
            var externalEventId = "AutomatedEventTestIdSuccessfulSave";
            var externalTimeStamp = DateTime.UtcNow.AddMinutes(-10);
            var participantId = seededConference.GetParticipants().First().Id;
            RoomType? transferredFrom = RoomType.WaitingRoom;
            RoomType? transferredTo = RoomType.ConsultationRoom1;
            var reason = "Automated";
            var eventType = EventType.Disconnected;

            var command = new SaveEventCommand(_newConferenceId, externalEventId, eventType, externalTimeStamp,
                participantId, transferredFrom, transferredTo, reason);
            await _handler.Handle(command);

            Event savedEvent;
            using (var db = new VideoApiDbContext(VideoBookingsDbContextOptions))
            {
                savedEvent = await db.Events.FirstOrDefaultAsync(x =>
                    x.ExternalEventId == externalEventId && x.ParticipantId == participantId);
            }

            savedEvent.Should().NotBeNull();
            savedEvent.ExternalEventId.Should().Be(externalEventId);
            savedEvent.EventType.Should().Be(eventType);
            savedEvent.ExternalTimestamp.Should().Be(externalTimeStamp);
            savedEvent.TransferredFrom.Should().Be(transferredFrom);
            savedEvent.TransferredTo.Should().Be(transferredTo);
            savedEvent.Reason.Should().Be(reason);
        }
    }
}