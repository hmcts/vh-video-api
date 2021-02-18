using System;
using System.Linq;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using Testing.Common.Helper.Builders.Domain;
using VideoApi.DAL;
using VideoApi.DAL.Commands;
using VideoApi.Domain;
using VideoApi.Domain.Enums;
using Task = System.Threading.Tasks.Task;

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
        public async Task Should_save_event()
        {
            var seededConference = await TestDataManager.SeedConference();
            TestContext.WriteLine($"New seeded conference id: {seededConference.Id}");
            _newConferenceId = seededConference.Id;

            var externalEventId = "AutomatedEventTestIdSuccessfulSave";
            var externalTimeStamp = DateTime.UtcNow.AddMinutes(-10);
            var participantId = seededConference.GetParticipants().First().Id;
            RoomType? transferredFrom = RoomType.WaitingRoom;
            RoomType? transferredTo = RoomType.ConsultationRoom;
            var reason = "Automated";
            var eventType = EventType.Disconnected;

            var command = new SaveEventCommand(_newConferenceId, externalEventId, eventType, externalTimeStamp,
                transferredFrom, transferredTo, reason, null) {ParticipantId = participantId};
            await _handler.Handle(command);

            Event savedEvent;
            await using (var db = new VideoApiDbContext(VideoBookingsDbContextOptions))
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
            savedEvent.EndpointFlag.Should().BeFalse();
            savedEvent.Phone.Should().BeNull();
        }

        [Test]
        public async Task Should_save_event_with_phone()
        {
            var seededConference = await TestDataManager.SeedConference();
            TestContext.WriteLine($"New seeded conference id: {seededConference.Id}");
            _newConferenceId = seededConference.Id;

            var externalEventId = "AutomatedEventTestIdSuccessfulSave";
            var externalTimeStamp = DateTime.UtcNow.AddMinutes(-10);
            var participantId = seededConference.GetParticipants().First().Id;
            RoomType? transferredFrom = RoomType.WaitingRoom;
            RoomType? transferredTo = RoomType.ConsultationRoom;
            var reason = "Automated";
            var eventType = EventType.Disconnected;
            var phone = "anonymous";

            var command = new SaveEventCommand(_newConferenceId, externalEventId, eventType, externalTimeStamp,
                transferredFrom, transferredTo, reason, phone)
            { ParticipantId = participantId };
            await _handler.Handle(command);

            Event savedEvent;
            await using (var db = new VideoApiDbContext(VideoBookingsDbContextOptions))
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
            savedEvent.EndpointFlag.Should().BeFalse();
            savedEvent.Phone.Should().Be(command.Phone);
        }

        [Test]
        public async Task should_set_endpoint_flag_to_true_when_endpoint_event()
        {
            var conference = new ConferenceBuilder()
                .WithEndpoint("Display1", "sip@123.com")
                .WithEndpoint("Display2", "sip@321.com").Build();
            
            var seededConference = await TestDataManager.SeedConference(conference);
            TestContext.WriteLine($"New seeded conference id: {seededConference.Id}");
            _newConferenceId = seededConference.Id;
            
            var externalEventId = "AutomatedEventTestIdSuccessfulSave";
            var externalTimeStamp = DateTime.UtcNow.AddMinutes(-10);
            var participantId = seededConference.GetEndpoints().First().Id;
            var reason = "Automated";
            var eventType = EventType.EndpointJoined;
            
            var command = new SaveEventCommand(_newConferenceId, externalEventId, eventType, externalTimeStamp,
                null, null, reason, null) {ParticipantId = participantId};
            await _handler.Handle(command);

            Event savedEvent;
            await using (var db = new VideoApiDbContext(VideoBookingsDbContextOptions))
            {
                savedEvent = await db.Events.FirstOrDefaultAsync(x =>
                    x.ExternalEventId == externalEventId && x.ParticipantId == participantId);
            }

            savedEvent.Should().NotBeNull();
            savedEvent.ExternalEventId.Should().Be(externalEventId);
            savedEvent.EventType.Should().Be(eventType);
            savedEvent.ExternalTimestamp.Should().Be(externalTimeStamp);
            savedEvent.TransferredFrom.Should().BeNull();
            savedEvent.TransferredTo.Should().BeNull();
            savedEvent.Reason.Should().Be(reason);
            savedEvent.ParticipantId.Should().Be(participantId);
            savedEvent.EndpointFlag.Should().BeTrue();
        }
    }
}
