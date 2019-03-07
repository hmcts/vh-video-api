using System;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using Testing.Common.Helper.Builders.Domain;
using VideoApi.DAL.Queries;
using VideoApi.Domain;
using VideoApi.Domain.Enums;
using VideoApi.Events.Exceptions;
using VideoApi.Events.Handlers;
using VideoApi.Events.Models;

namespace VideoApi.UnitTests.Events
{
    public class HelpEventHandlerTests : EventHandlerTestBase
    {
        private HelpEventHandler _eventHandler;

        [Test]
        public async Task should_send_messages_to_participants_and_service_bus_on_disconnect()
        {
            _eventHandler = new HelpEventHandler(QueryHandlerMock.Object, ServiceBusQueueClient,
                EventHubContextMock.Object);

            var conference = TestConference;
            var participantForEvent = conference.GetParticipants().First();
            var callbackEvent = new CallbackEvent
            {
                EventType = EventType.Help,
                EventId = Guid.NewGuid().ToString(),
                ParticipantId = participantForEvent.Id.ToString(),
                ConferenceId = conference.Id.ToString()
            };

            await _eventHandler.HandleAsync(callbackEvent);

            // Verify messages sent to event hub clients
            EventHubClientMock.Verify(
                x => x.HelpMessage(conference.HearingRefId, participantForEvent.DisplayName), Times.Once);
        }
        
        [Test]
        public void should_throw_exception_when_no_officer_assigned_to_hearing()
        {
            var conferenceWithoutOfficer = new ConferenceBuilder()
                .WithParticipant(UserRole.Judge, null)
                .WithParticipant(UserRole.Individual, "Claimant")
                .WithParticipant(UserRole.Representative, "Claimant")
                .WithParticipant(UserRole.Individual, "Defendant")
                .WithParticipant(UserRole.Representative, "Defendant")
                .Build();
            
            QueryHandlerMock
                .Setup(x => x.Handle<GetConferenceByIdQuery, Conference>(It.IsAny<GetConferenceByIdQuery>()))
                .ReturnsAsync(conferenceWithoutOfficer);
            
            _eventHandler = new HelpEventHandler(QueryHandlerMock.Object, ServiceBusQueueClient,
                EventHubContextMock.Object);

            var conference = TestConference;
            var participantForEvent = conference.GetParticipants().First();
            var callbackEvent = new CallbackEvent
            {
                EventType = EventType.Help,
                EventId = Guid.NewGuid().ToString(),
                ParticipantId = participantForEvent.Id.ToString(),
                ConferenceId = conference.Id.ToString()
            };

            Assert.ThrowsAsync<VideoHearingOfficeNotFoundException>(() =>
                _eventHandler.HandleAsync(callbackEvent));
            
            // Verify messages sent to event hub clients
            EventHubClientMock.Verify(
                x => x.HelpMessage(conference.HearingRefId, participantForEvent.DisplayName), Times.Never);
        }
    }
}