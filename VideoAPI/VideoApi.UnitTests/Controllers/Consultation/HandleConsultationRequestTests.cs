using System.Linq;
using Microsoft.AspNetCore.SignalR;
using Moq;
using NUnit.Framework;
using Testing.Common.Helper.Builders.Domain;
using Video.API.Controllers;
using VideoApi.Contract.Requests;
using VideoApi.DAL.Commands;
using VideoApi.DAL.Commands.Core;
using VideoApi.DAL.Queries;
using VideoApi.DAL.Queries.Core;
using VideoApi.Domain;
using VideoApi.Domain.Enums;
using VideoApi.Events.Hub;
using Task = System.Threading.Tasks.Task;

namespace VideoApi.UnitTests.Controllers.Consultation
{
    public class HandleConsultationRequestTests
    {
        private Mock<ICommandHandler> _commandHandlerMock;
        private ConsultationController _controller;
        private Mock<IEventHubClient> _eventHubClientMock;
        private Mock<IHubContext<EventHub, IEventHubClient>> _hubContextMock;
        private Mock<IQueryHandler> _queryHandlerMock;

        private Conference _testConference;

        [SetUp]
        public void Setup()
        {
            _queryHandlerMock = new Mock<IQueryHandler>();
            _commandHandlerMock = new Mock<ICommandHandler>();
            _hubContextMock = new Mock<IHubContext<EventHub, IEventHubClient>>();
            _eventHubClientMock = new Mock<IEventHubClient>();

            _testConference = new ConferenceBuilder()
                .WithParticipant(UserRole.Judge, null)
                .WithParticipant(UserRole.VideoHearingsOfficer, null)
                .WithParticipant(UserRole.Individual, "Claimant")
                .WithParticipant(UserRole.Representative, "Claimant")
                .WithParticipant(UserRole.Individual, "Defendant")
                .WithParticipant(UserRole.Representative, "Defendant")
                .Build();

            _queryHandlerMock
                .Setup(x => x.Handle<GetConferenceByIdQuery, Conference>(It.IsAny<GetConferenceByIdQuery>()))
                .ReturnsAsync(_testConference);

            _commandHandlerMock
                .Setup(x => x.Handle(It.IsAny<SaveEventCommand>()))
                .Returns(Task.FromResult(default(object)));

            _controller = new ConsultationController(_queryHandlerMock.Object, _commandHandlerMock.Object,
                _hubContextMock.Object);

            foreach (var participant in _testConference.GetParticipants())
                _hubContextMock.Setup(x => x.Clients.Group(participant.Username.ToString()))
                    .Returns(_eventHubClientMock.Object);
        }

        [Test]
        public async Task should_raise_notification_to_requestee_when_consultation_is_requested()
        {
            var conferenceId = _testConference.Id;
            var requestedBy = _testConference.GetParticipants()[2];
            var requestedFor = _testConference.GetParticipants()[3];
            var admin = _testConference.GetParticipants().First(x => x.UserRole == UserRole.VideoHearingsOfficer);

            var request = new ConsultationRequest
            {
                ConferenceId = conferenceId,
                RequestedBy = requestedBy.Id,
                RequestedFor = requestedFor.Id
            };
            await _controller.HandleConsultationRequest(request);

            _hubContextMock.Verify(x => x.Clients.Group(requestedBy.Username.ToLowerInvariant()), Times.Never);
            _hubContextMock.Verify(x => x.Clients.Group(requestedFor.Username.ToLowerInvariant()), Times.Once);
            _hubContextMock.Verify(x => x.Clients.Group(admin.Username.ToLowerInvariant()), Times.Never);

            _eventHubClientMock.Verify(
                x => x.ConsultationMessage(conferenceId, requestedBy.Username, requestedFor.Username, string.Empty),
                Times.Once);
        }

        [Test]
        public async Task should_raise_notification_to_requester_consultation_is_rejected()
        {
            var conferenceId = _testConference.Id;
            var requestedBy = _testConference.GetParticipants()[2];
            var requestedFor = _testConference.GetParticipants()[3];
            var answer = ConsultationAnswer.Rejected;
            var admin = _testConference.GetParticipants().First(x => x.UserRole == UserRole.VideoHearingsOfficer);

            var request = new ConsultationRequest
            {
                ConferenceId = conferenceId,
                RequestedBy = requestedBy.Id,
                RequestedFor = requestedFor.Id,
                Answer = answer
            };

            await _controller.HandleConsultationRequest(request);

            _hubContextMock.Verify(x => x.Clients.Group(requestedBy.Username.ToLowerInvariant()), Times.Once);
            _hubContextMock.Verify(x => x.Clients.Group(requestedFor.Username.ToLowerInvariant()), Times.Never);
            _hubContextMock.Verify(x => x.Clients.Group(admin.Username.ToLowerInvariant()), Times.Never);

            _eventHubClientMock.Verify(
                x => x.ConsultationMessage(conferenceId, requestedBy.Username, requestedFor.Username,
                    answer.ToString()), Times.Once);
        }

        [Test]
        public async Task should_raise_notification_to_requester_and_admin_when_consultation_is_accepted()
        {
            var conferenceId = _testConference.Id;
            var requestedBy = _testConference.GetParticipants()[2];
            var requestedFor = _testConference.GetParticipants()[3];
            var admin = _testConference.GetParticipants().First(x => x.UserRole == UserRole.VideoHearingsOfficer);

            var answer = ConsultationAnswer.Accepted;

            var request = new ConsultationRequest
            {
                ConferenceId = conferenceId,
                RequestedBy = requestedBy.Id,
                RequestedFor = requestedFor.Id,
                Answer = answer
            };

            await _controller.HandleConsultationRequest(request);

            _hubContextMock.Verify(x => x.Clients.Group(requestedBy.Username.ToLowerInvariant()), Times.Once);
            _hubContextMock.Verify(x => x.Clients.Group(requestedFor.Username.ToLowerInvariant()), Times.Never);
            _hubContextMock.Verify(x => x.Clients.Group(admin.Username.ToLowerInvariant()), Times.Once);
            _eventHubClientMock.Verify(
                x => x.ConsultationMessage(conferenceId, requestedBy.Username, requestedFor.Username,
                    answer.ToString()), Times.Exactly(2));
        }
    }
}